using System;

namespace Jacdac
{
    public delegate void PacketReceivedEvent(ITransport sender, Packet packet);

    public enum TransportError
    {
        Frame = 0,
        Overrun = 1,
        BufferFull = 2,
    }

    public sealed class TransportErrorReceivedEventArgs
    {
        public TransportError Error { get; }
        public DateTime Timestamp { get; }

        public byte[] Data { get; }

        public TransportErrorReceivedEventArgs(TransportError error, DateTime timestamp, byte[] data)
        {
            this.Error = error;
            this.Timestamp = timestamp;
            this.Data = data;
        }
    }

    public delegate void TransportErrorReceivedEvent(ITransport sender, TransportErrorReceivedEventArgs args);

    public interface ITransport : IDisposable
    {
        /// <summary>
        /// Sends a packet over the transport
        /// </summary>
        /// <param name="packet"></param>
        void SendPacket(Packet packet);

        /// <summary>
        /// Raised when a packet is received
        /// </summary>
        event PacketReceivedEvent PacketReceived;

        /// <summary>
        /// Raised when an error is received
        /// </summary>
        event TransportErrorReceivedEvent ErrorReceived;
    }
}
