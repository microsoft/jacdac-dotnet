using Jacdac.Services;
using Jacdac.Transport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Jacdac.Transport.USBTransport;

namespace Jacdac.Playground
{
    class Program
    {
        // Service: CODAL Message Bus
        const int SRV_CODAL_MESSAGE_BUS = 0x121ff81d;
        enum CodalMessageBusCmd : ushort
        {
            /**
             * Send a message on the CODAL bus. If `source` is `0`, it is treated as wildcard.
             *
             * ```
             * const [source, value] = jdunpack<[number, number]>(buf, "u16 u16")
             * ```
             */
            Send = 0x80,
        }

        enum CodalMessageBusEvent
        {
            /**
             * Raised by the server is triggered by the server. The filtering logic of which event to send over JACDAC is up to the server implementation.
             *
             * ```
             * const [source, value] = jdunpack<[number, number]>(buf, "u16 u16")
             * ```
             */
            //% block="message"
            Message = 0x80,
        }

        static void Main(string[] args)
        {
            AsyncMain();
        }

        static async void AsyncMain() {
            Console.WriteLine("Jacdac.Playground");

            var usbDevices = USBTransport.GetDevices();
            var usbDevice = usbDevices.FirstOrDefault();
            Console.WriteLine("connecting to " + usbDevice.DeviceName);
            var transport = new USBTransport(usbDevice);
            await transport.Connect();
            var bus = new JDBus(transport);

            Console.WriteLine("connected... connect codal message bus devices");
            bus.OnDeviceConnect += (deviceId, device) =>
            {
                Console.WriteLine("device " + deviceId + " connected");
                if (device == null) return;

                var messageBus = device.Services.FirstOrDefault(srv => srv.ServiceId == SRV_CODAL_MESSAGE_BUS);
                if (messageBus == null) return;

                Console.WriteLine("found message bus");

                var timer = new System.Threading.Timer(async (cb) =>
                {
                    Console.WriteLine("send");
                    byte[] data = { 0xe8, 0x03, 0x01, 0x00 };
                    var pkt = messageBus.BuildAction((ushort)CodalMessageBusCmd.Send, data);
                    await device.SendPacket(pkt);
                }, null, 0, 1000);
            };
        }
    }
}
