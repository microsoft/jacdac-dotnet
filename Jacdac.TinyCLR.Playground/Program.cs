
using GHIElectronics.TinyCLR.Devices.Uart;
using GHIElectronics.TinyCLR.Pins;
using System.Diagnostics;
using System.Threading;
using Jacdac;
using System;
using Jacdac.Servers;
using GHIElectronics.TinyCLR.Devices.Jacdac.Transport;
using Jacdac.Transports;
using Jacdac.Storage;
using GHIElectronics.TinyCLR.Devices.Storage;
using GHIElectronics.TinyCLR.Devices.Gpio;
using Jacdac.Clients;

namespace Jacdac_RgbLed
{
    internal class Program
    {
        static void Main()
        {
            new Program().Start();
        }

        // ServiceTwins serviceTwins;
        public void Start()
        {
            // Display enable
            Display.Enable();

            var sdStorage = new StorageManager(StorageController.FromName(SC20100.StorageController.SdCard));
            var ssidStorage = sdStorage.MountSettingsStorage("wifi.json");
            var specStorage = sdStorage.MountSpecificationStorage("services");

            // start wifi
            Display.WriteLine("Start wifi....");
            var wifiServer = new WifiServer(ssidStorage);
            wifiServer.ScanStarted += WifiServer_ScanStarted;
            wifiServer.ScanCompleted += WifiServer_ScanCompleted;
            wifiServer.Ssid.Changed += this.Ssid_Changed;
            wifiServer.Start();

            Thread.Sleep(5000);

            // jacdac
            Display.WriteLine("Configuration Jacdac....");
            Platform.LedPin = FEZBit.GpioPin.Led;
            var transport = new UartTransport(new JacdacSerialWireController(SC20260.UartPort.Uart4, new UartSetting { SwapTxRxPin = true }));

            //var serviceStorage = sdStorage.MountKeyStorage("servicestwins.json");
            var rtc = new RealTimeClockServer(() => DateTime.Now, new RealTimeClockServerOptions { Variant = RealTimeClockVariant.Crystal });
            var settingsStorage = sdStorage.MountSettingsStorage("settings.json");
            var settingsServer = new SettingsServer(settingsStorage);
            var protoTest = new ProtoTestServer();
            var bus = new JDBus(transport, new JDBusOptions
            {
                Description = "TinyCLR Demo",
                FirmwareVersion = "0.0.0",
                Services = new JDServiceServer[] { rtc, protoTest, wifiServer, settingsServer },
                SpecificationCatalog = new ServiceSpecificationCatalog(specStorage)
            });
            bus.DeviceConnected += Bus_DeviceConnected;
            bus.DeviceDisconnected += Bus_DeviceDisconnected;
            bus.SelfAnnounced += Bus_SelfAnnounced;

            Display.WriteLine("cached specs:");
            foreach (var sc in bus.SpecificationCatalog.Storage.GetServiceClasses())
                Display.WriteLine("  0x" + sc.ToString("x1"));

            transport.FrameReceived += (Transport sender, byte[] frame) =>
            {
                //  Debug.WriteLine($"{bus.Timestamp.TotalMilliseconds}\t\t{HexEncoding.ToString(frame)}");
            };

            Display.WriteLine($"Self device: {bus.SelfDeviceServer}");
            bus.Start();

            var humidity = new HumidityClient(bus, "humidity");

            //Blink(transport);
            while (true)
            {
                try
                {

                    Display.WriteLine($"humidity: {humidity.Humidity}");
                }
                catch (ClientDisconnectedException)
                {
                    Display.WriteLine("connect humidity");
                }
                //Display.WriteLine($".");
                Thread.Sleep(1000);
            }
        }

        private static void WifiServer_ScanStarted(JDNode sender, EventArgs e)
        {
            Display.WriteLine($"Wifi: Scanning...");
        }

        private static void WifiServer_ScanCompleted(JDNode sender, EventArgs e)
        {
            var wifi = (WifiServer)sender;
            foreach (var ssid in wifi.LastScanResults)
                Display.WriteLine($"  {ssid}");
        }

        private void Ssid_Changed(JDNode sender, EventArgs e)
        {
            var wifi = (JDStaticRegisterServer)sender;
            var ssid = wifi.GetValueAsString();
            Display.WriteLine($"SSID: {ssid}");
        }

        private static void Bus_SelfAnnounced(JDNode sender, EventArgs e)
        {
            var bus = (JDBus)sender;
            var freeRam = GHIElectronics.TinyCLR.Native.Memory.ManagedMemory.FreeBytes;
            var usedRam = GHIElectronics.TinyCLR.Native.Memory.ManagedMemory.UsedBytes;
            //  Display.WriteLine($"d{bus.GetDevices().Length} s{TransportStats.FrameSent} r{TransportStats.FrameReceived} e{TransportStats.FrameError} {freeRam / 1000}kb");
            //  Debug.WriteLine($"d{bus.GetDevices().Length} s{TransportStats.FrameSent} r{TransportStats.FrameReceived} e{TransportStats.FrameError} A{TransportStats.FrameA} B{TransportStats.FrameB} C{TransportStats.FrameC} D{TransportStats.FrameD} E{TransportStats.FrameE} F{TransportStats.FrameF} Busy{TransportStats.FrameBusy} Full{TransportStats.BufferFull}");
        }

        private static void Bus_DeviceDisconnected(JDNode node, DeviceEventArgs e)
        {
            Display.WriteLine($"{e.Device} disconnected");
        }

        private void Bus_DeviceConnected(JDNode node, DeviceEventArgs e)
        {
            Display.WriteLine($"{e.Device} connected");
            var bus = (JDBus)node;
            var device = e.Device;
            device.Announced += (JDNode sender, System.EventArgs ev) =>
            {
                Display.WriteLine($"{e.Device} announced");
                if (e.Device.DeviceId == bus.SelfDeviceServer.DeviceId) Display.WriteLine("  self");
                if (e.Device.IsDashboard) Display.WriteLine($" dashboard");
                if (e.Device.IsUniqueBrain) Display.WriteLine($" unique brain");
                if (e.Device.IsBridge) Display.WriteLine($" bridge");

                device.Identify();
                try
                {
                    var uptimeReg = device.GetServices()[0].GetRegister((ushort)Jacdac.ControlReg.Uptime);
                    uptimeReg.SendGet();
                }
                catch (AckException)
                {
                    System.Diagnostics.Debug.WriteLine("ack missing");
                }

                foreach (var service in device.GetServices())
                {
                    service.ResolveSpecification();
                    if (service.ServiceIndex == 0) continue;
                    Display.WriteLine(service.ToString());

                    // attach to reading
                    var reading = service.GetRegister((ushort)Jacdac.SystemReg.Reading);
                    if (reading != null)
                    {
                        Display.WriteLine(reading.ToString());
                        reading.Changed += (reg, er) =>
                        {
                            var freeRam = GHIElectronics.TinyCLR.Native.Memory.ManagedMemory.FreeBytes;
                            var usedRam = GHIElectronics.TinyCLR.Native.Memory.ManagedMemory.UsedBytes;
                            // Display.WriteLine($"get {reading.Service.Device.ShortId}[{reading.Service.ServiceIndex}] {HexEncoding.ToString(reading.Data)} {usedRam / 1000} / {freeRam / 1000}kb");
                        };
                    }

                    // attach to active/inactive
                    var active = service.GetEvent((ushort)Jacdac.SystemEvent.Active, true);
                    active.Changed += (eve, evv) =>
                    {
                        Display.WriteLine($"active {active}: {active.Count}");
                    };
                    var inactive = service.GetEvent((ushort)Jacdac.SystemEvent.Inactive, true);
                    inactive.Changed += (eve, evv) =>
                    {
                        Display.WriteLine($"inactive {inactive}: {inactive.Count}");
                    };

                    // spec
                    //var spec = this.serviceTwins.ResolveSpecification(service.ServiceClass);
                    //Debug.WriteLine(spec?.ToString());
                }
            };
        }
    }
}
