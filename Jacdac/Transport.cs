using System;

namespace Jacdac
{
    public delegate void ConnectionStateChangedEvent(Transport sender, ConnectionState newState);

    public delegate void FrameReceivedEvent(Transport sender, byte[] frame);

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

    public delegate void TransportErrorReceivedEvent(Transport sender, TransportErrorReceivedEventArgs args);

    public enum ConnectionState
    {
        Connected,
        Connecting,
        Disconnecting,
        Disconnected,
    }

    public abstract class Transport : JDNode, IDisposable
    {
        private ConnectionState _connectionState = ConnectionState.Disconnected;

        protected Transport()
        {

        }

        protected void SetConnectionState(ConnectionState connectionState)
        {
            if (this._connectionState != connectionState)
            {
                this._connectionState = connectionState;
                if (null != this.ConnectionChanged)
                    this.ConnectionChanged.Invoke(this, this._connectionState);
            }
        }

        public ConnectionState ConnectionState
        {
            get { return this._connectionState; }
        }

        /// <summary>
        /// Sends data over the transport. First 2 bytes should be a Crc16
        /// </summary>
        public abstract void SendFrame(byte[] data);

        /// <summary>
        /// Connect to the transport
        /// </summary>
        public void Connect()
        {
            if (this._connectionState != ConnectionState.Disconnected)
                return;

            this.SetConnectionState(ConnectionState.Connecting);
            try
            {
                this.InternalConnect();
                this.SetConnectionState(ConnectionState.Connected);
            }
            catch (Exception)
            {
                this.SetConnectionState(ConnectionState.Disconnected);
                throw;
            }
        }

        protected abstract void InternalConnect();

        /// <summary>
        /// Disconnects from the transport
        /// </summary>
        public void Disconnect()
        {
            if (this._connectionState != ConnectionState.Connected)
                return;

            this.SetConnectionState(ConnectionState.Disconnecting);
            try
            {
                this.InternalDisconnect();
                this.SetConnectionState(ConnectionState.Disconnected);
            }
            catch (Exception)
            {
                this.SetConnectionState(ConnectionState.Disconnected);
                throw;
            }
        }

        protected abstract void InternalDisconnect();

        public virtual void Dispose() { }

        public abstract event FrameReceivedEvent FrameReceived;

        /// <summary>
        /// Raised when an error is received
        /// </summary>
        public abstract event TransportErrorReceivedEvent ErrorReceived;

        /// <summary>
        /// Raised when the connection state of the transport changed
        /// </summary>
        public event ConnectionStateChangedEvent ConnectionChanged;
    }
}
