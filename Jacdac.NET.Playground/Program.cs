using Jacdac.Clients;
using Jacdac.Samples;
using Jacdac.Servers;
using Jacdac.Transports;
using Jacdac.Transports.Spi;
using Jacdac.Transports.WebSockets;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Jacdac.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("jacdac: connecting...");

            var prototest = args.Any(arg => arg == "prototest");
            var sample = SampleExtensions.GetSample(args);
            // create and start bus
            var bus = new JDBus(null, new JDBusOptions()
            {
                DisableRoleManager = sample == null,
                Services = prototest ? new JDServiceServer[] { new ProtoTestServer() } : null,
                SpecificationCatalog = new ServiceSpecificationCatalog()
            });
            bus.DeviceConnected += (s, e) =>
            {
                Console.WriteLine($"device connected: {e.Device}");
                e.Device.Announced += (s2, e2) =>
                {
                    foreach (var service in e.Device.GetServices())
                        service.ResolveSpecification();
                };
                e.Device.Restarted += (s2, e2) => Console.WriteLine($"device restarted: {e.Device}");
            };

            bus.DeviceDisconnected += (s, e) => Console.WriteLine($"device disconnected: {e.Device}");
            if (bus.RoleManager != null)
            {
                bus.RoleManager.Connected += (s, e) => Console.WriteLine($"roles connected");
                bus.RoleManager.Disconnected += (s, e) => Console.WriteLine($"roles connected");
            }

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

            // stats
            new Timer(state =>
            {
                Console.WriteLine(bus.Describe());
            }, null, 1000, 15000);

            //  run test
            if (sample != null)
                new Thread(state => sample.Run(bus)).Start();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
