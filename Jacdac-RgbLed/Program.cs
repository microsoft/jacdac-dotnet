
using GHIElectronics.TinyCLR.Devices.Uart;
using GHIElectronics.TinyCLR.Pins;
using System.Diagnostics;
using System.Threading;
using Jacdac;
using System;

namespace Jacdac_RgbLed
{
    internal class Program
    {
        static void Main()
        {
            // Display enable
            Display.Enable();


            DoTestJacdacBlink();

            //DoTestJacdacAdafruit();
        }

        static void DoTestJacdacBlink()
        {
            // jacdac
            Display.WriteLine("Configuration Jacdac....");
            var transport = new UartTransport(new GHIElectronics.TinyCLR.Devices.Jacdac.JacdacController(SC20260.UartPort.Uart4, new UartSetting { SwapTxRxPin = true }));
            transport.FrameReceived += Transport_FrameReceived;
            transport.ErrorReceived += JacdacController_ErrorReceived;

            var bus = new JDBus(transport);
            bus.DeviceConnected += Bus_DeviceConnected;
            bus.DeviceDisconnected += Bus_DeviceDisconnected;

            Display.WriteLine($"Self device: {bus.SelfDevice}");
            //Blink(transport);
            while(true)
            {
                //Display.WriteLine($".");
                Thread.Sleep(10000);
            }
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
                if (e.Device == bus.SelfDevice) Display.WriteLine("  self");
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
                        Display.WriteLine($"reading {reading}: {reading.Data}");
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


        private static void Blink(UartTransport transport)
        {
            var ledOnPacket = Packet.FromBinary(new byte[] { 0xa3, 0x2f, 0x08, 0x01, 0x46, 0x2e, 0xcd, 0xca, 0x66, 0xca, 0x4d, 0x19, 0x04, 0x01, 0x80, 0x00, 0x7f, 0x7f, 0x7f, 0x0 });
            var ledOffPacket = Packet.FromBinary(new byte[] { 0x9c, 0xa8, 0x08, 0x01, 0x46, 0x2e, 0xcd, 0xca, 0x66, 0xca, 0x4d, 0x19, 0x04, 0x01, 0x80, 0x00, 0x00, 0x00, 0x00, 0x0 });

            Display.WriteLine("led is blinking...");

            while (true)
            {
                transport.SendPacket(ledOnPacket);

                Thread.Sleep(250);

                transport.SendPacket(ledOffPacket);
                Thread.Sleep(250);
            }
        }

        private static void JacdacController_ErrorReceived(Transport sender, TransportErrorReceivedEventArgs args)
        {
            switch (args.Error)
            {
                case TransportError.Frame:
                    if (args.Data != null)
                    {
                        var str = "Frame error: ";

                        for (var i = 0; i < args.Data.Length; i++)
                        {
                            str += args.Data[i].ToString("x2");
                        }

                        Debug.WriteLine(str);
                    }
                    break;

                case TransportError.BufferFull:
                    (sender as UartTransport).controller.ClearReadBuffer();
                    Debug.WriteLine("Buffer full");
                    break;

                case TransportError.Overrun:
                    Debug.WriteLine("Overrun");
                    break;


            }            
        }

        private static void Transport_FrameReceived(Transport sender, byte[] frame, TimeSpan timestamp)
        {
            Debug.WriteLine($"{timestamp.TotalMilliseconds}\t\t{HexEncoding.ToString(frame)}");
        }


        private static void JacdacController_PacketReceived(Transport sender, Packet packet)
        {
            Debug.WriteLine("=>>>>>>>>>>>>>>>>>>>>>>>>>>");            
            Debug.WriteLine("packet crc             = " + packet.Crc.ToString("x2"));
            Debug.WriteLine("device_identifier      = " + packet.DeviceId);
            //Debug.WriteLine("size                   = " + packet.Size);
            //Debug.WriteLine("frame_flags            = " + packet.FrameFlags);
            Debug.WriteLine("is_command             = " + packet.IsCommand);
            Debug.WriteLine("service_command        = " + packet.ServiceCommand.ToString("x2"));
            Debug.WriteLine("get                    = " + packet.IsRegisterGet);
            Debug.WriteLine("set                    = " + packet.IsRegisterSet);
            //Debug.WriteLine("is_report              = " + packet.IsReport);
            //Debug.WriteLine("multicommand_class     = " + packet.MulticommandClass.ToString("x2"));
            //Debug.WriteLine("requires_ack           = " + packet.IsRequiresAck);
            //Debug.WriteLine("isEvent                = " + packet.IsEvent);
            //Debug.WriteLine("event_code             = " + packet.EventCode.ToString("x2"));
            //Debug.WriteLine("reg_code               = " + packet.RegisterCode.ToString("x2"));
            //Debug.WriteLine("eventCounter           = " + packet.EventCounter.ToString("x2"));
            Debug.WriteLine(packet.ToString());
            Debug.WriteLine(" ");
        }
    }


}
