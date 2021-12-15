
using GHIElectronics.TinyCLR.Devices.Uart;
using GHIElectronics.TinyCLR.Pins;
using System.Diagnostics;
using System.Threading;
using Jacdac;
using System;
using Jacdac.Servers;

namespace Jacdac_RgbLed
{
    internal class Program
    {
        static void Main()
        {
            // Display enable
            Display.Enable();
            // jacdac
            Display.WriteLine("Configuration Jacdac....");
            var transport = new UartTransport(new GHIElectronics.TinyCLR.Devices.Jacdac.JacdacController(SC20260.UartPort.Uart4, new UartSetting { SwapTxRxPin = true }));

            var ssidStorage = new SdCardKeyStorage("wifi.json");
            var rtc = new RealTimeClockServer(() => DateTime.Now, RealTimeClockVariant.Crystal);
            var wifiServer = new WifiServer(ssidStorage);
            var bus = new JDBus(transport, new JDBusOptions
            {
                Description = "TinyCLR Demo",
                FirmwareVersion = "0.0.0",
                Services = new JDServiceServer[] { rtc, wifiServer }
            });
            bus.DeviceConnected += Bus_DeviceConnected;
            bus.DeviceDisconnected += Bus_DeviceDisconnected;
            bus.SelfAnnounced += Bus_SelfAnnounced;
            wifiServer.ScanCompleted += WifiServer_ScanCompleted;
            wifiServer.Ssid.Changed += Ssid_Changed;
            transport.FrameReceived += (Transport sender, byte[] frame) =>
            {
                Debug.WriteLine($"{bus.Timestamp.TotalMilliseconds}\t\t{HexEncoding.ToString(frame)}");
            };

            Display.WriteLine($"Self device: {bus.SelfDeviceServer}");
            bus.Start();
            //Blink(transport);
            while (true)
            {
                //Display.WriteLine($".");
                Thread.Sleep(10000);
            }
        }

        private static void WifiServer_ScanCompleted(JDNode sender, EventArgs e)
        {
            var wifi = (WifiServer)sender;
            Display.WriteLine($"Wifi: Scan completed");
            foreach (var ssid in wifi.LastScanResults)
                Display.WriteLine($"  {ssid}");
        }

        private static void Ssid_Changed(JDNode sender, EventArgs e)
        {
            var wifi = (JDStaticRegisterServer)sender;
            var ssid = wifi.GetValues()[0];
            Display.WriteLine($"wifi: {ssid}");
        }

        private static void Bus_SelfAnnounced(JDNode sender, EventArgs e)
        {
            Debug.WriteLine($"self announced");
        }

        private static void Bus_DeviceDisconnected(JDNode node, DeviceEventArgs e)
        {
            Display.WriteLine($"{e.Device} disconnected");
        }

        private static void Bus_DeviceConnected(JDNode node, DeviceEventArgs e)
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
                foreach (var service in device.Services())
                {
                    if (service.ServiceIndex == 0) continue;
                    Display.WriteLine($" {service.ServiceIndex}: x{service.ServiceClass.ToString("x2")}");

                    // attach to reading
                    var reading = service.GetRegister((ushort)Jacdac.SystemReg.Reading, true);
                    reading.Changed += (reg, er) =>
                    {
                        Display.WriteLine($"get {reading}: {HexEncoding.ToString(reading.Data)}");
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
                }
            };
        }
    }
}
