#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
using Device.Net;
using Usb.Net.Windows;
using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Jacdac.Transports.Usb
{
    [Serializable]
    public sealed class USBTransportOptions
    {
        public string DeviceId;
    }

    public sealed class UsbTransport : Transport
    {
        private readonly USBTransportOptions Options;
        IDeviceFactory deviceManager;
        ILoggerFactory loggerFactory;

        /*
        public static USBDeviceDescription[] GetDevices()
        {
            var devices = new List<USBDeviceDescription>();
            foreach (var device in LibUsbDotNet.UsbDevice.AllWinUsbDevices.ToArray())
            {
                UsbDevice potentialDevice;
                if (device.Open(out potentialDevice))
                {
                    if (potentialDevice.Configs.Any(c => c.InterfaceInfoList.Any(i => i.Descriptor.Class == LibUsbDotNet.Descriptors.ClassCodeType.VendorSpec && i.Descriptor.SubClass == 42)))
                    {
                        potentialDevice.Close();
                        var descr = new USBDeviceDescription
                        {
                            DeviceName = device.FullName,
                            DeviceID = ((WinUsbRegistry)device).DeviceID,
                            VID = device.Vid,
                            PID = device.Pid
                        };
                        devices.Add(descr);
                        Debug.WriteLine($"usb: found {descr.DeviceID}");
                    }
                    potentialDevice.Close();
                }
            }
            return devices.ToArray();
        }
        */
        public static readonly List<FilterDeviceDefinition> UsbDeviceDefinitions = new List<FilterDeviceDefinition>
        {
            new FilterDeviceDefinition(),
        };

        private ConnectedDeviceDefinition usbDevice;
        private UsbHF2Transport transport;

        public static UsbTransport Create(USBTransportOptions options = null)
        {
            if (options == null)
                options = new USBTransportOptions();
            return new UsbTransport(options);
        }

        internal UsbTransport(USBTransportOptions options)
            : base("usb")
        {
            this.Options = options;
            this.loggerFactory = LoggerFactory.Create((builder) => builder.AddDebug());
            this.deviceManager = new List<IDeviceFactory>
            {
                UsbDeviceDefinitions.CreateWindowsUsbDeviceFactory(loggerFactory)
            }.Aggregate(loggerFactory);
        }

        public override event FrameEventHandler FrameReceived;
        public override event TransportErrorReceivedEvent ErrorReceived;

        public override void SendFrame(byte[] data)
        {
            if (this.ConnectionState == ConnectionState.Connected)
                this.transport.SendFrame(data);
        }

        private void handleHF2FrameReceived(byte[] frame)
        {
            this.FrameReceived?.Invoke(this, frame);
        }

        protected override void InternalConnect()
        {
            var devicesTask = this.deviceManager.GetConnectedDeviceDefinitionsAsync();
            devicesTask.Wait();
            var usbDevices = devicesTask.Result
                .OrderBy(d => d.Manufacturer)
                .ThenBy(d => d.ProductName)
                .ToArray();
            foreach (var device in usbDevices)
            {
                Console.WriteLine(device);
            }

            Debug.WriteLine($"usb: found {usbDevices.Length} devices");
            var deviceId = this.Options.DeviceId;
            this.usbDevice = usbDevices.FirstOrDefault(d => deviceId == null || deviceId == d.DeviceId);
            if (this.usbDevice == null)
            {
                this.SetConnectionState(ConnectionState.Disconnected);
                return;
            }
            Console.WriteLine($"usb: connecting {this.usbDevice.DeviceId}");
            var deviceTask = this.deviceManager.GetDeviceAsync(this.usbDevice);
            deviceTask.Wait();
            var device = deviceTask.Result;

            var initTask = device.InitializeAsync();
            initTask.Wait();

            var transport = new UsbHF2Transport(device, this.handleHF2FrameReceived);
            var connectionTask = transport.Connect();
            connectionTask.ContinueWith(prev =>
            {
                if (prev.IsFaulted)
                    this.SetConnectionState(ConnectionState.Disconnected);
                else
                {
                    this.transport = transport;
                    this.SetConnectionState(ConnectionState.Connected);
                }
            });
        }

        protected override void InternalDisconnect()
        {
            this.usbDevice = null;
            var tr = this.transport;
            tr?.Close();
        }
    }
}