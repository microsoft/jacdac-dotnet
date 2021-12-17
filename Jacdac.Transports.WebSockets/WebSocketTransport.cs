using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;

namespace Jacdac.Transports.WebSockets
{
    public class WebSocketTransport : Transport
    {
        public readonly Uri Uri;
        public override event FrameReceivedEvent FrameReceived;
        public override event TransportErrorReceivedEvent ErrorReceived;

        private ClientWebSocket socket;
        private SemaphoreSlim sendSemaphore;

        public WebSocketTransport(Uri uri = null)
        {
            this.Uri = uri ?? new Uri("ws://localhost:8081/");
            this.socket = new ClientWebSocket();
            this.sendSemaphore = new SemaphoreSlim(1);
        }

        public override void SendFrame(byte[] data)
        {
            if (this.ConnectionState != ConnectionState.Connected) return;

            this.sendSemaphore.WaitAsync()
                .ContinueWith(t =>
                {
                    if (!t.IsFaulted)
                        this.socket.SendAsync(data, WebSocketMessageType.Binary, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);
                    this.sendSemaphore.Release();
                });
        }

        protected override void InternalConnect()
        {
            this.socket.ConnectAsync(this.Uri, CancellationToken.None)
                .ContinueWith(prev =>
                {
                    if (prev.IsFaulted)
                    {
                        Debug.WriteLine("websocket: connection failed");
                        this.SetConnectionState(ConnectionState.Disconnected);
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
            while (this.socket.State == WebSocketState.Open
                && this.ConnectionState == ConnectionState.Connected)
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
            if (this.ConnectionState == ConnectionState.Connected)
                this.SetConnectionState(ConnectionState.Disconnected);
        }

        protected override void InternalDisconnect()
        {
            this.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "user cancelled", CancellationToken.None);
        }
    }
}