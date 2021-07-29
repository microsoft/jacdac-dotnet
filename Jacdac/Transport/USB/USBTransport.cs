using Jacdac;
using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.Main;
using LibUsbDotNet.WinUsb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Jacdac.JDPacket;

namespace Jacdac.Transport
{ 

    public class USBTransport : JDTransport
    {
        public struct USBDeviceDescription
        {
            public string DeviceID { get; set; }
            public string DeviceName { get; set; }
            public int VID { get; set; }
            public int PID { get; set; }
        }

        public static USBDeviceDescription[] GetDevices()
        {
            List<USBDeviceDescription> devices = new List<USBDeviceDescription>();
            foreach (var device in UsbDevice.AllWinUsbDevices.ToArray())
            {
                UsbDevice potentialDevice;
                if (device.Open(out potentialDevice))
                {
                    if(potentialDevice.Configs.Any(c => c.InterfaceInfoList.Any(i => i.Descriptor.Class == LibUsbDotNet.Descriptors.ClassCodeType.VendorSpec && i.Descriptor.SubClass == 42)))
                    {
                        potentialDevice.Close();
                        devices.Add(new USBDeviceDescription
                        {
                            DeviceName = device.FullName,
                            DeviceID = ((WinUsbRegistry)device).DeviceID,
                            VID = device.Vid,
                            PID = device.Pid
                        });
                    }
                    potentialDevice.Close();
                }
            }
            return devices.ToArray();
        }

        private USBDeviceDescription deviceDescription;
        private UsbDevice usbDevice;
        private UsbEndpointWriter writer;
        private HF2 hf2;
        private SemaphoreSlim sendSemaphore = new SemaphoreSlim(1);

        public USBTransport(USBDeviceDescription deviceDescription)
        {
            this.deviceDescription = deviceDescription;
        }

        public async Task<bool> Connect()
        {
            
            var device = UsbDevice.AllWinUsbDevices.FirstOrDefault(d => d.Vid == deviceDescription.VID && d.Pid == deviceDescription.PID);
            if (device == null)
                throw new Exception("Device not found");

            if (device.Open(out usbDevice))
            {
                hf2 = new HF2(this);
                    
                var endpoints = usbDevice.Configs[0].InterfaceInfoList[0].EndpointInfoList;

                var inEndpoint = endpoints.First(e => e.Descriptor.EndpointID >> 7 == 1);
                var outEndpoint = endpoints.First(e => e.Descriptor.EndpointID >> 7 == 0);

                var reader = usbDevice.OpenEndpointReader((ReadEndpointID)inEndpoint.Descriptor.EndpointID, 64, EndpointType.Bulk);
                reader.DataReceived += Reader_DataReceived;
                reader.DataReceivedEnabled = true;

                writer = usbDevice.OpenEndpointWriter((WriteEndpointID)outEndpoint.Descriptor.EndpointID, EndpointType.Bulk);
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

            hf2.OnJacdacMessage += Hf2_OnJacdacMessage;

            await hf2.SendCommand(0x20, new byte[1] { 1 });
            return true;

        }

        public void Close()
        {
            usbDevice.Close();
        }

        #region Frame Handling
        private void Hf2_OnJacdacMessage(byte[] data)
        {
            InternalReceiveFrame(new JDFrame(data));
        }

        protected override async Task InternalSendFrame(JDFrame frame)
        {
            await hf2.SendCommand(0x0021, frame.Bytes, false);
        }
        #endregion

        #region USB Communication
        internal async Task SendData(byte[] buffer)
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
        #endregion
    }
}
