using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Network;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace Jacdac_RgbLed
{
    internal class WiFi
    {
        static public void Enable(string wifi_ssid, string wifi_pass)
        {
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

            var networkController = NetworkController.FromName(WIFI_API_NAME);

            WiFiNetworkInterfaceSettings networkInterfaceSetting = new WiFiNetworkInterfaceSettings()
            {
                Ssid = wifi_ssid,
                Password = wifi_pass,
            };

            var networkReady = false;

            networkInterfaceSetting.DhcpEnable = true;
            networkInterfaceSetting.DynamicDnsEnable = true;

            networkController.SetCommunicationInterfaceSettings(networkCommunicationInterfaceSettings);
            networkController.SetInterfaceSettings(networkInterfaceSetting);
            networkController.SetAsDefaultController();

            networkController.NetworkAddressChanged += (sender, args) =>
            {
                var ipProperties = sender.GetIPProperties();

                var address = ipProperties.Address.GetAddressBytes();

                if (address[0] != 0 && address[1] != 0)
                    networkReady = true;
            };

            networkController.Enable();

            while (networkReady == false) ;



        }
    }
}
