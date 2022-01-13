using Jacdac;
using Jacdac.Transports.Spi;
using Microsoft.AspNetCore.Connections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

var internet = args.Any(arg => arg == "--internet");
var spi = args.Any(arg => arg == "--spi");
var stats = args.Any(arg => arg == "--stats");
var host = internet ? "*" : "localhost";
var port = 8081;
var url = $"http://{host}:{port}";

Console.WriteLine("Jacdac DevTools (.NET)");
Console.WriteLine("");
Console.WriteLine("  --spi       enable SPI transport");
Console.WriteLine("  --internet  bind all network interfaces");
Console.WriteLine("  --stats     show various stats");
Console.WriteLine("");
Console.WriteLine($"   dashboard: {url}");
Console.WriteLine($"   websocket: ws://{host}:{port}");
if (internet)
{
    Console.WriteLine("WARNING: all network interfaces bound");
    var server = Dns.GetHostName();
    var heserver = Dns.GetHostEntry(server);
    foreach (var ip in heserver.AddressList)
        Console.WriteLine($"  {ip}");
}

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

JDBus? bus = null;
if (spi)
{
    Console.WriteLine("starting Jacdac bus...");
    var spiTransport = SpiTransport.Create();
    bus = new JDBus(spiTransport, new JDBusOptions
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
    if (stats)
    {
        new Timer(state =>
        {
            Console.Write(bus);
            Console.WriteLine(spiTransport);
        }, null, 0, 30000);
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
        {
            clients.Add(ws);
            Console.WriteLine($"clients: {clients.Count} connected");
        }
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
                    {
                        clients.Remove(ws);
                        Console.WriteLine($"clients: {clients.Count} connected");
                    }
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
app.Run(url);
