using Jacdac.Transports;
using Jacdac.Transports.WebSockets;
using System;
using System.Diagnostics;

namespace Jacdac.NET.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            NETPlatform.Init();
            Console.WriteLine("jacdac: connecting...");
            var bus = new JDBus(null);
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                switch (arg)
                {
                    case "spi":
                        Console.WriteLine("adding spi connection");
                        bus.AddTransport(Jacdac.Transports.Spi.SpiTransport.CreateRaspberryPiJacdapterTransport());
                        break;
                    case "devtools":
                        Console.WriteLine("adding devtools connection");
                        bus.AddTransport(new WebSocketTransport());
                        break;
                }
            }
            foreach (var transport in bus.Transports)
            {
                transport.ConnectionChanged += (sender, newState) =>
                {
                    Console.WriteLine($"{sender.Kind}: {newState}");
                };
            }
            bus.DeviceConnected += (sender, conn) =>
            {
                var device = conn.Device;
                var selfMsg = bus.SelfDeviceServer.DeviceId == device.DeviceId ? "(self)" : "";
                Console.WriteLine($"device connected {device} {selfMsg}");

                device.Announced += (sender, an) =>
                {
                    var services = device.GetServices();
                    foreach (var service in services)
                    {
                        Console.WriteLine(service);
                    }
                };
            };
            bus.Start();

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
