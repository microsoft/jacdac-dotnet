
using GHIElectronics.TinyCLR.Devices.Uart;
using GHIElectronics.TinyCLR.Pins;
using System.Diagnostics;
using System.Threading;
using Jacdac;
using System;
using Jacdac.Servers;
using GHIElectronics.TinyCLR.Devices.Jacdac.Transport;

namespace Jacdac_RgbLed
{
    internal class Program
    {
        static void Main()
        {
            new Program().Start();
        }

        ServiceTwins serviceTwins;
        public void Start()
        {
            // Display enable
            Display.Enable();
            // jacdac
            Display.WriteLine("Configuration Jacdac....");

            // configure FEZbit status light with 1 LED
            Jacdac.Platform.StatusLight = ControlAnnounceFlags.StatusLightMono;
            Jacdac.Platform.SetStatusLight = (red, green, blue, speed) =>
            {
                var on = red > 0;
                // TODO: handle light
            };

            var transport = new UartTransport(new JacdacSerialWireController(SC20260.UartPort.Uart4, new UartSetting { SwapTxRxPin = true }));

            var sdStorage = new SdCardKeyStorage();
            var ssidStorage = sdStorage.MountKeyStorage("wifi.json");
            var serviceStorage = sdStorage.MountKeyStorage("servicestwins.json");
            var settingsStorage = sdStorage.MountKeyStorage("settings.json");
            this.serviceTwins = new ServiceTwins(serviceStorage);

            var rtc = new RealTimeClockServer(() => DateTime.Now, new RealTimeClockServerOptions { Variant = RealTimeClockVariant.Crystal });
            var wifiServer = new WifiServer(ssidStorage);
            var settingsServer = new SettingsServer(settingsStorage);
            var protoTest = new ProtoTestServer();
            var bus = new JDBus(transport, new JDBusOptions
            {
                Description = "TinyCLR Demo",
                FirmwareVersion = "0.0.0",
                Services = new JDServiceServer[] { rtc, protoTest, wifiServer, settingsServer }
            });
            bus.DeviceConnected += Bus_DeviceConnected;
            bus.DeviceDisconnected += Bus_DeviceDisconnected;
            bus.SelfAnnounced += Bus_SelfAnnounced;
            wifiServer.ScanStarted += WifiServer_ScanStarted;
            wifiServer.ScanCompleted += WifiServer_ScanCompleted;
            wifiServer.Ssid.Changed += this.Ssid_Changed;
            transport.FrameReceived += (Transport sender, byte[] frame) =>
            {
                //  Debug.WriteLine($"{bus.Timestamp.TotalMilliseconds}\t\t{HexEncoding.ToString(frame)}");
            };

            Display.WriteLine($"Self device: {bus.SelfDeviceServer}");
            bus.Start();
            //wifiServer.Start();

            //Blink(transport);
            while (true)
            {
                //Display.WriteLine($".");
                Thread.Sleep(200);
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
            Display.WriteLine($"s{TransportStats.FrameSent} r{TransportStats.FrameReceived} e{TransportStats.FrameError} {freeRam / 1000}kb");
            Debug.WriteLine($"d{bus.GetDevices().Length} s{TransportStats.FrameSent} r{TransportStats.FrameReceived} e{TransportStats.FrameError} A{TransportStats.FrameA} B{TransportStats.FrameB} C{TransportStats.FrameC} D{TransportStats.FrameD} E{TransportStats.FrameE} F{TransportStats.FrameF} Busy{TransportStats.FrameBusy} Full{TransportStats.BufferFull}");
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

                if (device.StatusLight != null)
                    device.StatusLight.Blink(0xff0000, 0, 500, 10);
                var uptimeReg = device.GetServices()[0].GetRegister((ushort)Jacdac.ControlReg.Uptime);
                try
                {
                    uptimeReg.SendGet(true);
                }
                catch (AckException)
                {
                    System.Diagnostics.Debug.WriteLine("ack missing");
                }

                foreach (var service in device.GetServices())
                {
                    if (service.ServiceIndex == 0) continue;
                    Display.WriteLine($" {service.ServiceIndex}: x{service.ServiceClass.ToString("x2")}");

                    // attach to reading
                    var reading = service.GetRegister((ushort)Jacdac.SystemReg.Reading);
                    reading.Changed += (reg, er) =>
                    {
                        var freeRam = GHIElectronics.TinyCLR.Native.Memory.ManagedMemory.FreeBytes;
                        var usedRam = GHIElectronics.TinyCLR.Native.Memory.ManagedMemory.UsedBytes;
                        // Display.WriteLine($"get {reading.Service.Device.ShortId}[{reading.Service.ServiceIndex}] {HexEncoding.ToString(reading.Data)} {usedRam / 1000} / {freeRam / 1000}kb");
                    };

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
