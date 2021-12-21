#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
using LibUsbDotNet;
using LibUsbDotNet.WinUsb;
using System.Diagnostics;

namespace Jacdac.Transports.Usb
{
    public sealed class USBDeviceDescription
    {
        public string DeviceID;
        public string DeviceName;
        public int VID;
        public int PID;
    }

    public class UsbTransport : Transport
    {
        public static USBDeviceDescription[] GetDevices()
        {
            List<USBDeviceDescription> devices = new List<USBDeviceDescription>();
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

        private USBDeviceDescription usbDevice;
        private UsbHF2Transport transport;

        public UsbTransport()
            : base()
        {
        }

        public override event FrameReceivedEvent FrameReceived;
        public override event TransportErrorReceivedEvent ErrorReceived;

        public override void SendFrame(byte[] data)
        {
            if (this.ConnectionState == ConnectionState.Connected)
                this.transport.SendFrame(data);
        }

        private void handleHF2FrameReceived(byte[] frame)
        {
            if (this.FrameReceived != null)
                this.FrameReceived.Invoke(this, frame);
        }

        public USBDeviceDescription UsbDevice
        {
            get => this.usbDevice;
        }


        protected override void InternalConnect()
        {
            var usbDevices = UsbTransport.GetDevices();
            Console.WriteLine($"usb: found ${usbDevices.Length} devices");
            this.usbDevice = usbDevices.FirstOrDefault();
            if (this.usbDevice == null)
            {
                this.SetConnectionState(ConnectionState.Disconnected);
                return;
            }
            Console.WriteLine($"usb: connecting {this.usbDevice.DeviceID}");
            var transport = new UsbHF2Transport(usbDevice, this.handleHF2FrameReceived);
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