using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Network;
using GHIElectronics.TinyCLR.Devices.Storage;
using GHIElectronics.TinyCLR.Drivers.Microchip.Winc15x0;
using GHIElectronics.TinyCLR.IO;
using GHIElectronics.TinyCLR.Pins;

namespace Jacdac.Servers
{
    public sealed class WifiServer : JDServiceServer
    {
        public readonly IKeyStorage KeyStorage;
        public readonly JDStaticRegisterServer Enabled;
        public readonly JDStaticRegisterServer IpAddress;
        public readonly JDStaticRegisterServer Eui48;
        public readonly JDStaticRegisterServer Ssid;

        private NetworkController networkController;
        private bool scanning = false;
        private string[] lastScanResults = null;

        public WifiServer(IKeyStorage keyStorage, JDServiceServerOptions options = null)
            : base(Jacdac.WifiConstants.ServiceClass, options)
        {
            this.KeyStorage = keyStorage;
            this.AddRegister(this.Enabled = new JDStaticRegisterServer(
                (ushort)Jacdac.WifiReg.Enabled, "u8", new object[] { (byte)0 }
                ));
            this.AddRegister(this.IpAddress = new JDStaticRegisterServer(
                (ushort)Jacdac.WifiReg.IpAddress, "b", new object[] { new byte[0] }
                ));
            this.AddRegister(this.Eui48 = new JDStaticRegisterServer(
                (ushort)Jacdac.WifiReg.Eui48, "b", new object[] { new byte[0] }
                ));
            this.AddRegister(this.Ssid = new JDStaticRegisterServer(
                (ushort)Jacdac.WifiReg.Ssid, "s", new object[] { "" }
                ));
            // TODO RSSi

            this.AddCommand((ushort)Jacdac.WifiCmd.AddNetwork, this.handleAddNetwork);
            this.AddCommand((ushort)Jacdac.WifiCmd.ForgetNetwork, this.handleForgetNetwork);
            this.AddCommand((ushort)Jacdac.WifiCmd.ForgetAllNetworks, this.handleForgetAllNetworks);
            this.AddCommand((ushort)Jacdac.WifiCmd.Scan, this.handleScan);
            this.AddCommand((ushort)Jacdac.WifiCmd.LastScanResults, this.handleLastScanResults);
            this.AddCommand((ushort)Jacdac.WifiCmd.ListKnownNetworks, this.handleListKnownNetworks);
            this.AddCommand((ushort)Jacdac.WifiCmd.Reconnect, this.handleReconnect);

            this.Enabled.Changed += Enabled_Changed;
            this.Ssid.Changed += Ssid_Changed;
            this.ScanCompleted += WifiServer_ScanCompleted;
        }

        private void WifiServer_ScanCompleted(JDNode sender, EventArgs e)
        {
            var enabled = this.Enabled.GetValueAsBool();
            if (enabled)
                new Thread(this.Connect).Start();
        }

        private void Ssid_Changed(JDNode sender, EventArgs e)
        {
            var ssid = (string)this.Ssid.GetValues()[0];
            var ev = !String.IsNullOrEmpty(ssid) ? (ushort)Jacdac.WifiEvent.GotIp : (ushort)Jacdac.WifiEvent.LostIp;
            this.SendEvent(ev);
        }

        private void Enabled_Changed(JDNode sender, EventArgs e)
        {
            var enabled = this.Enabled.GetValueAsBool();
            if (enabled)
                this.StartScan();
            else
                this.Stop();
        }

        public void Start()
        {
            this.Enabled.SetValues(new object[] { (byte)1 });
        }

        public void Stop()
        {
            if (this.networkController == null) return;

            this.networkController.Disable();
            this.networkController.Dispose();
            this.networkController = null;

            this.Ssid.SetValues(new object[] { "" });
            this.IpAddress.SetValues(new object[] { new byte[0] });
            this.Enabled.SetValues(new object[] { (byte)0 });
        }

        private void handleReconnect(JDNode node, PacketEventArgs args)
        {
            this.Connect();
        }

        private void handleListKnownNetworks(JDNode node, PacketEventArgs args)
        {
            var device = this.Device;
            if (device == null) return;

            new Thread(() =>
            {
                var pipe = OutPipe.From(device.Bus, args.Packet);
                pipe?.RespondForEach(this.KeyStorage.GetKeys(), (result) =>
                {
                    var key = (string)result;
                    return PacketEncoding.Pack("i16 i16 s", new object[] { 0, 0, key });
                });
            }).Start();
        }

        private void handleLastScanResults(JDNode node, PacketEventArgs args)
        {
            var device = this.Device;
            if (device == null) return;

            new Thread(() =>
            {
                var pipe = OutPipe.From(device.Bus, args.Packet);
                var ssids = this.lastScanResults ?? new string[0];
                pipe?.RespondForEach(ssids, (result) =>
                {
                    var ssid = (string)result;
                    return PacketEncoding.Pack("u32 u32 i8 u8 u8[6] s", new object[] {
                        0u,
                        0u, // reserved
                        0u, // RSSI
                        0u, // channel
                        new byte[6], // bssid
                        ssid
                    });
                });
            }).Start();
        }

        private void handleScan(JDNode node, PacketEventArgs args)
        {
            var enabled = this.Enabled.GetValueAsBool();
            if (enabled)
                this.StartScan();
        }

        private void initController()
        {
            if (this.networkController != null) return;

            const string WIFI_API_NAME = "GHIElectronics.TinyCLR.NativeApis.ATWINC15xx.NetworkController";

            const int RESET = FEZBit.GpioPin.WiFiReset;
            const int SPI_CS = FEZBit.GpioPin.WiFiChipselect;
            const int SPI_INT = FEZBit.GpioPin.WiFiInterrupt;
            const int ENABLE = FEZBit.GpioPin.WiFiEnable;

            var gpioController = GpioController.GetDefault();

            var interrupt = gpioController.OpenPin(SPI_INT);
            var reset = gpioController.OpenPin(RESET);
            var cs = gpioController.OpenPin(SPI_CS);
            var en = gpioController.OpenPin(ENABLE);

            var networkCommunicationInterfaceSettings = new SpiNetworkCommunicationInterfaceSettings();
            var settings = new GHIElectronics.TinyCLR.Devices.Spi.SpiConnectionSettings()
            {
                ChipSelectLine = cs,
                ClockFrequency = 2000000,
                Mode = GHIElectronics.TinyCLR.Devices.Spi.SpiMode.Mode0,
                ChipSelectType = GHIElectronics.TinyCLR.Devices.Spi.SpiChipSelectType.Gpio,
                ChipSelectHoldTime = TimeSpan.FromTicks(10),
                ChipSelectSetupTime = TimeSpan.FromTicks(10)
            };

            networkCommunicationInterfaceSettings.SpiApiName = FEZBit.SpiBus.WiFi;
            networkCommunicationInterfaceSettings.GpioApiName = SC20100.GpioPin.Id;
            networkCommunicationInterfaceSettings.SpiSettings = settings;
            networkCommunicationInterfaceSettings.InterruptPin = interrupt;
            networkCommunicationInterfaceSettings.InterruptEdge = GpioPinEdge.FallingEdge;
            networkCommunicationInterfaceSettings.InterruptDriveMode = GpioPinDriveMode.InputPullUp;
            networkCommunicationInterfaceSettings.ResetPin = reset;
            networkCommunicationInterfaceSettings.ResetActiveState = GpioPinValue.Low;

            en.SetDriveMode(GpioPinDriveMode.Output);
            en.Write(GpioPinValue.High);

            this.networkController = NetworkController.FromName(WIFI_API_NAME);
            this.networkController.SetCommunicationInterfaceSettings(networkCommunicationInterfaceSettings);
        }

        public string[] LastScanResults
        {
            get { return this.lastScanResults != null ? (string[])this.lastScanResults.Clone() : new string[0]; }
        }

        public void StartScan()
        {
            if (this.scanning) return;


            this.scanning = true;
            this.ScanStarted?.Invoke(this, EventArgs.Empty);

            new Thread(() =>
            {
                try
                {
                    Debug.WriteLine("Wifi: scanning...");
                    this.initController();
                    var ssids = Winc15x0Interface.Scan();
                    this.lastScanResults = ssids;

                    var knownSsids = this.KeyStorage.GetKeys();
                    uint total = (uint)ssids.Length;
                    uint known = 0;
                    for (var i = 0; i < ssids.Length; i++)
                        if (Array.IndexOf(knownSsids, ssids[i]) != -1)
                            known++;

                    Debug.WriteLine($"Wifi: found {total} ssids, {known} known");

                    this.SendEvent(
                        (ushort)Jacdac.WifiEvent.ScanComplete,
                        PacketEncoding.Pack(Jacdac.WifiEventPack.ScanComplete, new object[] { total, known })
                    );

                    this.ScanCompleted?.Invoke(this, EventArgs.Empty);
                }
                finally
                {
                    this.scanning = false;
                }
            }).Start();
        }

        public event NodeEventHandler ScanStarted;
        public event NodeEventHandler ScanCompleted;

        private void handleAddNetwork(JDNode node, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var values = PacketEncoding.UnPack(Jacdac.WifiCmdPack.AddNetwork, pkt.Data);
            if (values == null) return;

            var ssid = (string)values[0];
            if (string.IsNullOrEmpty(ssid)) return;

            var password = (string)values[1];
            this.AddNetwork(ssid, password);
        }

        public void AddNetwork(string ssid, string password)
        {
            if (string.IsNullOrEmpty(ssid))
                throw new ArgumentNullException("ssid");
            this.KeyStorage.Write(ssid, UTF8Encoding.UTF8.GetBytes(password));
            this.RaiseChanged();
        }

        private void handleForgetNetwork(JDNode node, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var values = PacketEncoding.UnPack(Jacdac.WifiCmdPack.ForgetNetwork, pkt.Data);
            if (values == null) return;
            var ssid = (string)values[0];
            if (!string.IsNullOrEmpty(ssid))
                this.KeyStorage.Delete(ssid);
            this.RaiseChanged();
        }

        private void handleForgetAllNetworks(JDNode node, PacketEventArgs args)
        {
            this.KeyStorage.Clear();
            this.RaiseChanged();
        }

        private void Connect()
        {
            if (this.networkController.ActiveInterfaceSettings != null)
            {
                Debug.WriteLine("Wifi: skip connect, already connecting");
                return;
            }

            // find best access point
            var secrets = this.FindAccessPoint();
            if (secrets == null)
            {
                Debug.WriteLine("Wifi: no known ssid found");
                return;
            }
            Debug.WriteLine($"Wifi: connecting to {secrets[0]}");

            var networkInterfaceSetting = new WiFiNetworkInterfaceSettings()
            {
                Ssid = secrets[0],
                Password = secrets[1],
            };

            networkInterfaceSetting.DhcpEnable = true;
            networkInterfaceSetting.DynamicDnsEnable = true;

            this.networkController.SetInterfaceSettings(networkInterfaceSetting);
            this.networkController.SetAsDefaultController();

            this.networkController.NetworkAddressChanged += (sender, args) =>
            {
                var ipProperties = sender.GetIPProperties();
                var address = ipProperties.Address.GetAddressBytes();

                if (address[0] != 0 && address[1] != 0)
                {
                    this.Ssid.SetValues(new object[] { networkInterfaceSetting.Ssid });
                    this.IpAddress.SetValues(new object[] { address });
                    Debug.WriteLine($"Wifi {networkInterfaceSetting.Ssid} connected");
                }
            };
            this.networkController.Enable();
        }

        private string[] FindAccessPoint()
        {
            if (this.lastScanResults == null) return null;

            var keys = this.KeyStorage.GetKeys();
            for (var i = 0; i < this.lastScanResults.Length; i++)
            {
                var ssid = this.lastScanResults[i];
                if (Array.IndexOf(keys, ssid) != -1)
                {
                    var buffer = this.KeyStorage.Read(ssid);
                    var password = UTF8Encoding.UTF8.GetString(buffer);
                    return new string[] { ssid, password };
                }
            }

            return null;
        }
    }
}
