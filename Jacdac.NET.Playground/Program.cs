using Jacdac.Transports;
using System;
using System.Diagnostics;

namespace Jacdac.NET.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.WriteLine("connecting usb");
            NETPlatform.Init();
            Transport transport;
            switch (args.Length > 0 ? args[0] : "ws")
            {
                case "spi": transport = Jacdac.Transports.Spi.SpiTransport.CreateRaspberryPiJacdapterTransport(); break;
                default:
                    transport = new Jacdac.Transports.WebSockets.WebSocketTransport();
                    break;
            }
            transport.ConnectionChanged += (sender, newState) =>
            {
                Console.WriteLine($"{sender.Kind}: {newState}");
            };

            var bus = new JDBus(transport);
            bus.DeviceConnected += (sender, conn) =>
            {
                var device = conn.Device;
                Console.WriteLine($"device connected {device}");

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
            transport.Connect();

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
