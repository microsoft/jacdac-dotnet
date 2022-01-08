using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jacdac.Transports.Hf2;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace Jacdac.Transports.Usb
{
    internal class UsbHF2Transport : IHf2Transport
    {
        private USBDeviceDescription deviceDescription;
        private UsbDevice usbDevice;
        private UsbEndpointWriter writer;
        private HF2 hf2;
        private SemaphoreSlim sendSemaphore = new SemaphoreSlim(1);
        private HF2.HF2EventArgs onFrameReceived;

        public UsbHF2Transport(USBDeviceDescription deviceDescription, HF2.HF2EventArgs onFrameReceived)
        {
            this.deviceDescription = deviceDescription;
            this.onFrameReceived = onFrameReceived;
        }

        public async void SendFrame(byte[] data)
        {
            await this.hf2.SendCommand(0x0021, data, false);
        }

        public async Task<bool> Connect()
        {

            var device = UsbDevice.AllWinUsbDevices.FirstOrDefault(d => d.Vid == deviceDescription.VID && d.Pid == deviceDescription.PID);
            if (device == null)
                throw new Exception("Device not found");

            if (device.Open(out usbDevice))
            {
                Debug.WriteLine("hf2: open device");
                hf2 = new HF2(this);

                var endpoints = usbDevice.Configs[0].InterfaceInfoList[0].EndpointInfoList;

                var inEndpoint = endpoints.First(e => e.Descriptor.EndpointID >> 7 == 1);
                var outEndpoint = endpoints.First(e => e.Descriptor.EndpointID >> 7 == 0);

                Debug.WriteLine("hf2: open reader");
                var reader = usbDevice.OpenEndpointReader((ReadEndpointID)inEndpoint.Descriptor.EndpointID, 64, EndpointType.Bulk);
                reader.DataReceived += Reader_DataReceived;
                reader.DataReceivedEnabled = true;

                Debug.WriteLine("hf2: open writer");
                this.writer = usbDevice.OpenEndpointWriter((WriteEndpointID)outEndpoint.Descriptor.EndpointID, EndpointType.Bulk);
            }
            else
            {
                throw new Exception("Could not open device");
            }

            if (await hf2.GetBootMode() == HF2.HF2BootMode.Bootloader)
            {
                Console.WriteLine("Device in bootloader - resetting");
                await hf2.ResetIntoApp();
                usbDevice.Close();
                throw new Exception("Device was in bootloader mode - reset");
            }

            hf2.OnJacdacMessage += this.onFrameReceived;

            Debug.WriteLine("hf2: send hf2 0x20 1");
            await hf2.SendCommand(0x20, new byte[1] { 1 });
            Debug.WriteLine("hf2: connected");
            return true;

        }

        public void Close()
        {
            Debug.WriteLine("hf2: close device");
            usbDevice.Close();
        }

        async Task IHf2Transport.SendData(byte[] buffer)
        {
            await sendSemaphore.WaitAsync();
            int bytesWritten;
            var err = writer.Write(buffer, 5000, out bytesWritten);
            if (err != ErrorCode.None)
                Console.WriteLine(err.ToString());
            sendSemaphore.Release();
        }

        private void Reader_DataReceived(object sender, EndpointDataEventArgs e)
        {
            hf2.ProcessPacket(e.Buffer);
        }
    }
}
