using System;
using System.Diagnostics;

namespace Jacdac
{
    public delegate void ConnectionStateChangedEvent(Transport sender, ConnectionState newState);

    public delegate void FrameEventHandler(Transport sender, byte[] frame);

    public enum TransportError : uint
    {
        NoError = 0u,
        Overrun = 1u,
        BufferFull = 2u,
        Frame = 0x80000000u,
        Frame_MaxData = 2147483649u,
        Frame_Busy = 2147483650u,
        Frame_A = 2147483652u,
        Frame_B = 2147483656u,
        Frame_C = 2147483664u,
        Frame_D = 2147483680u,
        Frame_E = 2147483712u,
        Frame_F = 2147483776u
    }

    /// <summary>
    /// Single wire serial stats
    /// https://microsoft.github.io/jacdac-docs/reference/single-wire-serial/
    /// </summary>
    public static class TransportStats
    {
        public static uint FrameReceived;
        public static uint FrameSent;
        public static uint FrameError;

        public static uint Overrun;
        public static uint BufferFull;
        public static uint Frame;
        public static uint FrameMaxData;
        public static uint FrameBusy;
        public static uint FrameA;
        public static uint FrameB;
        public static uint FrameC;
        public static uint FrameD;
        public static uint FrameE;
        public static uint FrameF;
    }

    public sealed class TransportErrorReceivedEventArgs
    {
        public TransportError Error { get; }
        public byte[] Data { get; }

        public TransportErrorReceivedEventArgs(TransportError error, byte[] data)
        {
            this.Error = error;
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

    /// <summary>
    /// Implements a Jacdac frame transport layer, which could be web sockets or single wire serial (https://microsoft.github.io/jacdac-docs/reference/single-wire-serial/).
    /// </summary>
    public abstract class Transport : IDisposable
    {
        public readonly string Kind;
        private ConnectionState _connectionState = ConnectionState.Disconnected;

        protected Transport(string kind)
        {
            this.Kind = kind;
        }

        public override string ToString()
        {
            return this.Kind;
        }

        /// <summary>
        /// Sets the connection state.
        /// </summary>
        /// <param name="connectionState"></param>
        protected void SetConnectionState(ConnectionState connectionState)
        {
            if (this._connectionState != connectionState)
            {
                this._connectionState = connectionState;
                if (null != this.ConnectionChanged)
                    this.ConnectionChanged.Invoke(this, this._connectionState);
            }
        }

        /// <summary>
        /// Gets the current connection state
        /// </summary>
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
                // connected state must be set by internal connect
            }
            catch (Exception)
            {
                this.Disconnect();
            }
        }

        /// <summary>
        /// Starts the connection process with the Jacdac Bus
        /// </summary>
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
        
        /// <summary>
        /// Starts the disconnection process
        /// </summary>
        protected abstract void InternalDisconnect();

        /// <summary>
        /// Release any native resources locked by this object
        /// </summary>
        public virtual void Dispose() { }

        /// <summary>
        /// Raised when a frame is received
        /// </summary>
        public abstract event FrameEventHandler FrameReceived;

        /// <summary>
        /// Raised when an error is received
        /// </summary>
        public abstract event TransportErrorReceivedEvent ErrorReceived;

        /// <summary>
        /// Raised when the connection state of the transport changed
        /// </summary>
        public event ConnectionStateChangedEvent ConnectionChanged;

        [Conditional("DEBUG")]
        protected void LogDebug(string msg)
        {
            Platform.LogDebug($"{this}: {msg}", "jacdac");
        }
    }
}
