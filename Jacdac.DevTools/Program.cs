using Jacdac;
using Jacdac.Transports.Spi;
using Microsoft.AspNetCore.Connections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
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
            DisableUniqueBrain = true,
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
        case "spi":
            {
                var b = CreateBus();
                b.AddTransport(SpiTransport.Create());
                break;
            }
    }
}

// download proxy code
var resp = await new HttpClient().GetAsync("https://microsoft.github.io/jacdac-docs/devtools/proxy");
resp.EnsureSuccessStatusCode();
var proxySource = await resp.Content.ReadAsStringAsync();

app.Lifetime.ApplicationStopping.Register(() =>
{
    lock (clients)
    {
        foreach (var client in clients)
        {
            try
            {
                client.Dispose();
            }
            catch { }
        }
        clients.Clear();
    }
});
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
                try
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
                }
                catch (SocketException)
                {

                }
                finally
                {
                    // web socket closed, clean
                    lock (clients)
                        clients.Remove(ws);
                    try
                    {

                    }
                    catch
                    {
                        ws.Dispose();
                    }
                }
            };
        await proxy();
    }
    else if (context.Request.Path == "/")
    {
        context.Response.Headers.ContentType = "text/html";
        context.Response.Headers.CacheControl = "no-cache";
        await context.Response.WriteAsync(proxySource);
        await context.Response.CompleteAsync();
    }
    else
    {
        await next();
    }

});
app.Run($"http://localhost:{port}/");
