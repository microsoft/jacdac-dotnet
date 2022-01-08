using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Jacdac.Transports.WebSockets
{
    public sealed class WebSocketTransport : Transport
    {
        const int RECONNECT_TIMEOUT = 5000;

        public readonly Uri Uri;
        public override event FrameReceivedEvent FrameReceived;
        public override event TransportErrorReceivedEvent ErrorReceived;

        private ClientWebSocket socket;
        private SemaphoreSlim sendSemaphore;
        private Timer reconnectTimer;

        public static WebSocketTransport Create(Uri uri = null)
        {
            return new WebSocketTransport(uri);
        }

        internal WebSocketTransport(Uri uri = null)
            : base("ws")
        {
            this.Uri = uri ?? new Uri("ws://localhost:8081/");
            this.sendSemaphore = new SemaphoreSlim(1);

            this.reconnectTimer = new Timer(this.handleReconnectTimer, null, RECONNECT_TIMEOUT, RECONNECT_TIMEOUT);
        }

        private void handleReconnectTimer(object sender)
        {
            if (this.ConnectionState == ConnectionState.Disconnected)
            {
                Console.WriteLine($"reconnect websocket {this.socket}");
                this.Connect();
            }
        }

        public override void SendFrame(byte[] data)
        {
            if (this.ConnectionState != ConnectionState.Connected) return;

            this.sendSemaphore.WaitAsync()
                .ContinueWith(t =>
                {
                    try
                    {
                        if (!t.IsFaulted && this.socket != null)
                            this.socket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);
                    }
                    finally
                    {
                        this.sendSemaphore.Release();
                    }
                });
        }

        protected override void InternalConnect()
        {
            Debug.Assert(this.socket == null);
            this.socket = new ClientWebSocket();
            this.socket.ConnectAsync(this.Uri, CancellationToken.None)
                .ContinueWith(prev =>
                {
                    if (prev.IsFaulted)
                    {
                        Debug.WriteLine("websocket: connection failed");
                        this.Disconnect();
                    }
                    else
                    {
                        Debug.WriteLine("websocket: connected");
                        this.SetConnectionState(ConnectionState.Connected);
                        Task.Run(this.ReadLoop);
                    }
                });
        }

        private async void ReadLoop()
        {
            try
            {
                while (this.socket.State == WebSocketState.Open
                    && this.ConnectionState == ConnectionState.Connected
                    && this.socket != null)
                {
                    var buffer = new byte[0xff];
                    var res = await this.socket.ReceiveAsync(buffer, CancellationToken.None);
                    if (res.Count > 0)
                    {
                        var frame = new byte[res.Count];
                        Array.Copy(buffer, 0, frame, 0, res.Count);
                        if (this.FrameReceived != null)
                            this.FrameReceived.Invoke(this, frame);
                    }
                }
            }
            catch (WebSocketException)
            {

            }
            finally
            {
                this.Disconnect();
            }
        }

        protected override void InternalDisconnect()
        {
            var socket = this.socket;
            if (socket != null)
            {
                this.socket = null;
                if (socket.State != WebSocketState.Closed)
                    socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None)
                        .ContinueWith(t => socket.Dispose());
                else socket.Dispose();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            var t = this.reconnectTimer;
            if (t != null)
            {
                this.reconnectTimer = null;
                t.Dispose();
            }
            var socket = this.socket;
            if (socket != null)
            {
                this.socket = null;
                if (socket.State != WebSocketState.Closed)
                    socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None)
                        .ContinueWith(t => socket.Dispose());
                else socket.Dispose();
            }
        }
    }
}