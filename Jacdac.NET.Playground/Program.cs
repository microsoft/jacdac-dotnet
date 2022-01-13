using Jacdac.Clients;
using Jacdac.Samples;
using Jacdac.Transports;
using Jacdac.Transports.Spi;
using Jacdac.Transports.WebSockets;
using System;
using System.Threading;

namespace Jacdac.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("jacdac: connecting...");

            var sample = SampleExtensions.GetSample(args);
            if (sample == null)
                throw new InvalidOperationException("please select a sample to run");

            // create and start bus
            var bus = new JDBus(null, new JDBusOptions()
            {
                SpecificationCatalog = new ServiceSpecificationCatalog()
            });
            bus.DeviceConnected += (s, e) => Console.WriteLine($"device connected: {e.Device}");
            bus.DeviceDisconnected += (s, e) => Console.WriteLine($"device disconnected: {e.Device}");
            bus.RoleManager.Connected += (s, e) => Console.WriteLine($"roles connected");
            bus.RoleManager.Disconnected += (s, e) => Console.WriteLine($"roles connected");

            // add transports
            for (var i = 0; i < args.Length; ++i)
            {
                var arg = args[i];
                switch (arg)
                {
                    case "spi":
                        Console.WriteLine("adding spi connection");
                        bus.AddTransport(SpiTransport.Create());
                        break;
                    case "devtools":
                        Console.WriteLine("adding devtools connection");
                        bus.AddTransport(WebSocketTransport.Create());
                        break;
                    case "ws":
                        var url = args[++i];
                        Console.WriteLine($"adding websocket connection to {url}");
                        bus.AddTransport(WebSocketTransport.Create(new Uri("ws://" + url)));
                        break;
                }
            }

            //  run test
            new Thread(state => sample.Run(bus)).Start();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
