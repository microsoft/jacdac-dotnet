using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Network;
using GHIElectronics.TinyCLR.Devices.Storage;
using GHIElectronics.TinyCLR.IO;
using GHIElectronics.TinyCLR.Pins;

namespace Jacdac.Servers
{
    public sealed class WifiServer : JDServiceServer
    {
        public readonly JDStaticRegisterServer Enabled;
        public readonly JDStaticRegisterServer IpAddress;
        public readonly JDStaticRegisterServer Eui48;
        public readonly JDStaticRegisterServer Ssid;

        private NetworkController networkController;

        public WifiServer()
            : base(Jacdac.WifiConstants.ServiceClass)
        {
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

            this.Enabled.Changed += Enabled_Changed;

            this.Start();
        }

        private void Enabled_Changed(JDNode sender, EventArgs e)
        {
            var values = this.Enabled.GetValues();
            var enabled = (byte)values[0] != 0 ? true : false;
            if (enabled)
                this.Start();
            else
                this.Stop();
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

        public void Start()
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
            var secrets = this.ReadSecrets();
            var networkInterfaceSetting = new WiFiNetworkInterfaceSettings()
            {
                Ssid = secrets[0],
                Password = secrets[1],
            };

            networkInterfaceSetting.DhcpEnable = true;
            networkInterfaceSetting.DynamicDnsEnable = true;

            this.networkController.SetCommunicationInterfaceSettings(networkCommunicationInterfaceSettings);
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
                    this.Enabled.SetValues(new object[] { (byte)1 });
                    Debug.WriteLine($"Wifi {networkInterfaceSetting.Ssid} connected");
                }
            };
            this.networkController.Enable();
        }

        private string[] ReadSecrets()
        {
            var sd = StorageController.FromName(SC20100.StorageController.SdCard);
            var drive = FileSystem.Mount(sd.Hdc);
            var bytes = System.IO.File.ReadAllBytes($@"{drive.Name}wifi.txt");
            var text = System.Text.Encoding.UTF8.GetString(bytes);
            var lines = text.Split('\n');
            FileSystem.Flush(sd.Hdc);
            return new string[] { lines[0].Trim(), lines[1].Trim() };
        }
    }
}
