using Jacdac;
using Jacdac.Transports.Spi;
using Jacdac.Transports.Usb;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;

var port = 8081;

Console.WriteLine("Jacdac DevTools");
Console.WriteLine($"   dashboard: http://localhost:{port}");
Console.WriteLine($"   websocket: ws://localhost:{port}");

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// better concurrent data structure?
var clients = new List<WebSocket>();
void SendFrame(WebSocket[] cs, byte[] frame)
{
    if (cs.Length == 0) return;
    Task.WaitAll(
        cs
        .Select(async (client) =>
        {
            try
            {
                await client.SendAsync(frame, WebSocketMessageType.Binary, true, CancellationToken.None);
            }
            catch
            {
                lock (clients)
                    clients.Remove(client);
                try
                {
                    client?.Dispose();
                }
                catch { }
            }
        }).ToArray());
}

// start bus if transport needed
JDBus? bus = null;
JDBus CreateBus()
{
    if (bus == null)
    {
        bus = new JDBus(null, new JDBusOptions
        {
            IsClient = false,
            IsInfrastructure = true,
            DisableBrain = true,
            DisableRoleManager = true,
            DisableLogger = true,
        });
        bus.DeviceConnected += (sender, device) => Debug.WriteLine($"{device.Device} connected");
        bus.DeviceDisconnected += (sender, device) => Debug.WriteLine($"{device.Device} disconnected");
        bus.FrameSent += (sender, frame) =>
        {
            WebSocket[] cs;
            lock (clients)
                cs = clients.ToArray();
            SendFrame(cs, frame);
        };
    }
    return bus;
}
foreach (var arg in args)
{
    switch (arg)
    {
        case "usb":
            {
                var b = CreateBus();
                b.AddTransport(UsbTransport.Create());
                break;
            }
        case "spi":
            {
                var b = CreateBus();
                b.AddTransport(SpiTransport.Create());
                break;
            }
    }
}

app.UseWebSockets();
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/" && context.WebSockets.IsWebSocketRequest)
    {
        var ws = await context.WebSockets.AcceptWebSocketAsync();
        lock (clients)
            clients.Add(ws);
        var proxy = async () =>
            {
                var buffer = new byte[512];
                while (ws.State == WebSocketState.Open)
                {
                    // grab frame
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var frame = buffer.Take(result.Count).ToArray();
                    // dispatch to bus
                    bus?.ProcessFrame(null, frame);
                    // dispatch to other clients
                    WebSocket[] cs;
                    lock (clients)
                        cs = clients.Where(client => client != ws).ToArray();
                    SendFrame(cs, frame);
                }
                // web socket closed, clean
                lock (clients)
                    clients.Remove(ws);
                ws.Dispose();
            };
        await proxy();
    }
    else
    {
        await next();
    }

});
app.UseDefaultFiles();
app.UseStaticFiles();
app.Run($"http://localhost:{port}/");
