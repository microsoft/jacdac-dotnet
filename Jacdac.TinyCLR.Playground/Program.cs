
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
using Jacdac.Samples;
using System.Collections;

namespace Jacdac.Playground
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

            var sampleName = "buttontracker";
            var sample = SampleExtensions.GetSample(new string[] { sampleName });
            if (sample == null)
                throw new InvalidOperationException("please select a sample to run");

            var sdStorage = new StorageManager(StorageController.FromName(SC20100.StorageController.SdCard));
            var ssidStorage = sdStorage.MountSettingsStorage("wifi.json");
            var specStorage = sdStorage.MountSpecificationStorage("services");
            var rolesStorage = sdStorage.MountSettingsStorage("roles.json");

            var servers = new ArrayList();

            // start wifi
            if (true)
            {
                Display.WriteLine("Start wifi....");
                var wifiServer = new WifiServer(ssidStorage);
                wifiServer.ScanStarted += (JDNode sender, EventArgs e) =>
                {
                    Display.WriteLine($"Wifi: Scanning...");
                };
                wifiServer.ScanCompleted += (JDNode sender, EventArgs e) =>
                {
                    var wifi = (WifiServer)sender;
                    foreach (var ssid in wifi.LastScanResults)
                        Display.WriteLine($"  {ssid}");
                };
                wifiServer.Ssid.Changed += (JDNode sender, EventArgs e) =>
                {
                    var wifi = (JDStaticRegisterServer)sender;
                    var ssid = wifi.GetValueAsString();
                    Display.WriteLine($"SSID: {ssid}");
                };
                wifiServer.Start();

                Thread.Sleep(5000);
                servers.Add(wifiServer);
            }

            // jacdac
            Display.WriteLine("Configuration Jacdac....");
            Platform.LedPin = FEZBit.GpioPin.Led;
            var transport = new UartTransport(new JacdacSerialWireController(SC20260.UartPort.Uart4, new UartSetting { SwapTxRxPin = true }));

            //var serviceStorage = sdStorage.MountKeyStorage("servicestwins.json");
            var rtc = new RealTimeClockServer(() => DateTime.Now, new RealTimeClockServerOptions());
            servers.Add(rtc);
            var settingsStorage = sdStorage.MountSettingsStorage("settings.json");
            var settings = new SettingsServer(settingsStorage);
            servers.Add(settings);
            //var protoTest = new ProtoTestServer();
            var bus = new JDBus(transport, new JDBusOptions
            {
                Description = "TinyCLR Demo",
                FirmwareVersion = "0.0.0",
                Services = servers.ToArray(typeof(JDServiceServer)) as JDServiceServer[],
                SpecificationCatalog = new ServiceSpecificationCatalog(specStorage),
                RoleStorage = rolesStorage,
                DefaultMinLoggerPriority = LoggerPriority.Log,
            });

            new Thread(() => sample.Run(bus)).Start();
            Thread.Sleep(Timeout.Infinite);
        }

    }
    /**
     *                         Display.WriteLine(reading.ToString());
                        reading.Changed += (reg, er) =>
                        {
                            var freeRam = GHIElectronics.TinyCLR.Native.Memory.ManagedMemory.FreeBytes;
                            var usedRam = GHIElectronics.TinyCLR.Native.Memory.ManagedMemory.UsedBytes;
                            // Display.WriteLine($"get {reading.Service.Device.ShortId}[{reading.Service.ServiceIndex}] {HexEncoding.ToString(reading.Data)} {usedRam / 1000} / {freeRam / 1000}kb");
                        };*/
}
