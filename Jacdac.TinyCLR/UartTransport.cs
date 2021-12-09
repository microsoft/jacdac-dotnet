using System;

namespace Jacdac
{
    public sealed class UartTransport : Transport
    {
        public readonly GHIElectronics.TinyCLR.Devices.Jacdac.JacdacController controller;

        public UartTransport(GHIElectronics.TinyCLR.Devices.Jacdac.JacdacController controller)
        {
            this.controller = controller;
        }

        public override event PacketReceivedEvent PacketReceived
        {
            add
            {
                this.controller.PacketReceived += (GHIElectronics.TinyCLR.Devices.Jacdac.JacdacController sender, GHIElectronics.TinyCLR.Devices.Jacdac.Packet packet) =>
                {
                    value(this, Packet.FromBinary(packet.ToBuffer()));
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

        protected override void SendData(byte[] data)
        {
            this.controller.SendData(data);
        }
    }
}