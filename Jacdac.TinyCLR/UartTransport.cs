using GHIElectronics.TinyCLR.Native;
using System;
using System.Runtime.InteropServices;

namespace Jacdac
{
    public sealed class UartTransport : Transport
    {
        static UartTransport()
        {
            Platform.Crc16 = GHIElectronics.TinyCLR.Devices.Jacdac.Util.CRC;
            var id = DeviceInformation.GetUniqueId();
            // TODO: compress device id into 8 bytes
            Platform.DeviceId = Util.Slice(id, 0, 8);
            Platform.DeviceDescription = DeviceInformation.DeviceName;
            var version = DeviceInformation.Version;
            var major = (ushort)((version >> 48) & 0xFFFF);
            var minor = (ushort)((version >> 32) & 0xFFFF);
            var build = (ushort)((version >> 16) & 0xFFFF);
            var revision = (ushort)((version >> 0) & 0xFFFF);
            Platform.FirmwareVersion = major + "." + minor + "." + build + "." + revision;
            Platform.RealTimeClock = RealTimeClockVariant.Crystal;
        }

        public readonly GHIElectronics.TinyCLR.Devices.Jacdac.JacdacController controller;

        public UartTransport(GHIElectronics.TinyCLR.Devices.Jacdac.JacdacController controller)
        {
            this.controller = controller;
        }

        public override event FrameReceivedEvent FrameReceived
        {
            add
            {
                this.controller.PacketReceived += (GHIElectronics.TinyCLR.Devices.Jacdac.JacdacController sender, GHIElectronics.TinyCLR.Devices.Jacdac.Packet packet) =>
                {
                    var pkt = Packet.FromBinary(packet.ToBuffer(), true);
                    var frame = Packet.ToFrame(new Packet[] { pkt });
                    value(this, frame);
                };
            }
            remove
            {
                throw new InvalidOperationException();
                // not supported
            }
        }


        public override event TransportErrorReceivedEvent ErrorReceived
        {
            add
            {
                this.controller.ErrorReceived += (GHIElectronics.TinyCLR.Devices.Jacdac.JacdacController sender, GHIElectronics.TinyCLR.Devices.Jacdac.ErrorReceivedEventArgs args) =>
                {
                    value(this, new TransportErrorReceivedEventArgs((TransportError)(uint)args.Error, args.Timestamp, args.Data));
                };
            }
            remove
            {
                throw new InvalidOperationException();
                // not supported
            }
        }

        protected override void InternalConnect()
        {
            this.controller.Enable();
        }

        protected override void InternalDisconnect()
        {
            this.controller.Disable();
        }

        public override void Dispose()
        {
            this.controller.Dispose();
        }

        public override void SendFrame(byte[] data)
        {
            this.controller.SendData(data);
        }
    }
}