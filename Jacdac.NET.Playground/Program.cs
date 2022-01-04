using Jacdac.Transports;
using Jacdac.Transports.WebSockets;
using System;

namespace Jacdac.NET.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("jacdac: connecting...");
            var bus = new JDBus(null);
            bus.IsStreaming = true;
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
                    case "twins":
                        Console.WriteLine("tracking twins");
                        bus.SpecificationCatalog = new ServiceSpecificationCatalog();
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
                        service.ResolveSpecification();
                        Console.WriteLine(service);
                        var reading = service.GetRegister((ushort)Jacdac.SystemReg.Reading);
                        if (reading != null)
                            reading.ReportReceived += (sender, rargs) =>
                            {
                                Console.Write($"  {reading}: ");
                                var values = reading.DeserializeValues();
                                foreach (var value in values)
                                    Console.Write($"{value}, ");
                                Console.WriteLine();
                            };
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
