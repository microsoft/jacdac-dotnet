
using GHIElectronics.TinyCLR.Devices.Uart;
using GHIElectronics.TinyCLR.Pins;
using System.Diagnostics;
using Jacdac;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Jacdac;

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
            var jacdacController = new JacdacController(SC20260.UartPort.Uart4, new UartSetting { SwapTxRxPin = true });

            // subscribe events
            jacdacController.PacketReceived += JacdacController_PacketReceived;
            jacdacController.ErrorReceived += JacdacController_ErrorReceived;

            // Jacdac enable
            jacdacController.Enable();

            // raw data packet
            var ledOnPacket = Packet.FromBinary(new byte[] { 0xa3, 0x2f, 0x08, 0x01, 0x46, 0x2e, 0xcd, 0xca, 0x66, 0xca, 0x4d, 0x19, 0x04, 0x01, 0x80, 0x00, 0x7f, 0x7f, 0x7f, 0x0 });
            var ledOffPacket = Packet.FromBinary(new byte[] { 0x9c, 0xa8, 0x08, 0x01, 0x46, 0x2e, 0xcd, 0xca, 0x66, 0xca, 0x4d, 0x19, 0x04, 0x01, 0x80, 0x00, 0x00, 0x00, 0x00, 0x0 });

            Display.WriteLine("led is blinking...");

            while (true)
            {
                jacdacController.SendPacket(ledOnPacket);

                Thread.Sleep(250);

                jacdacController.SendPacket(ledOffPacket);
                Thread.Sleep(250);
            }

        }
        private static void JacdacController_ErrorReceived(JacdacController sender, GHIElectronics.TinyCLR.Devices.Jacdac.ErrorReceivedEventArgs args)
        {
            switch (args.Error)
            {
                case JacdacError.Frame:
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

                case JacdacError.BufferFull:
                    sender.ClearReadBuffer();
                    Debug.WriteLine("Buffer full");
                    break;

                case JacdacError.Overrun:
                    Debug.WriteLine("Overrun");
                    break;


            }            
        }

        private static void JacdacController_PacketReceived(JacdacController sender, Packet packet)
        {
            Debug.WriteLine("=>>>>>>>>>>>>>>>>>>>>>>>>>> New packet >>>>>>>>>>>>>>>>>>>>>>>>");            
            Debug.WriteLine("packet crc             = " + packet.Crc.ToString("x2"));
            Debug.WriteLine("device_identifier      = " + packet.DeviceIdentifier);
            //Debug.WriteLine("size                   = " + packet.Size);
            //Debug.WriteLine("frame_flags            = " + packet.FrameFlags);
            //Debug.WriteLine("is_command             = " + packet.IsCommand);
            //Debug.WriteLine("is_report              = " + packet.IsReport);
            //Debug.WriteLine("multicommand_class     = " + packet.MulticommandClass.ToString("x2"));
            //Debug.WriteLine("requires_ack           = " + packet.IsRequiresAck);
            //Debug.WriteLine("isEvent                = " + packet.IsEvent);
            //Debug.WriteLine("event_code             = " + packet.EventCode.ToString("x2"));
            //Debug.WriteLine("reg_code               = " + packet.RegisterCode.ToString("x2"));
            //Debug.WriteLine("eventCounter           = " + packet.EventCounter.ToString("x2"));

            Debug.WriteLine(" ");
        }
    }


}
