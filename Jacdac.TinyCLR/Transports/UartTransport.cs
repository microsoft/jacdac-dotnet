using GHIElectronics.TinyCLR.Data.Json;
using GHIElectronics.TinyCLR.Native;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using GHIElectronics.TinyCLR.Devices.Jacdac.Transport;

namespace Jacdac.Transports
{
    public sealed partial class UartTransport : Jacdac.Transport
    {
        public readonly GHIElectronics.TinyCLR.Devices.Jacdac.Transport.JacdacSerialWireController controller;

        public UartTransport(GHIElectronics.TinyCLR.Devices.Jacdac.Transport.JacdacSerialWireController controller)
            : base("uart")
        {
            this.controller = controller;
        }

        public override event FrameEventHandler FrameReceived
        {
            add
            {
                this.controller.PacketReceived += (JacdacSerialWireController sender, PacketReceivedEventArgs packet) =>
                {
                    var frame = packet.Data;
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
                this.controller.ErrorReceived += (JacdacSerialWireController sender, ErrorReceivedEventArgs args) =>
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
            try
            {
                this.controller.Enable();
                this.SetConnectionState(ConnectionState.Connected);
            }
            catch (Exception)
            {
                this.SetConnectionState(ConnectionState.Disconnected);
            }
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
            TransportStats.FrameSent++;
            this.controller.Write(data);
        }
    }
}