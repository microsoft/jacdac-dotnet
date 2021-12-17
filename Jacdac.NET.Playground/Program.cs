using Jacdac.Transports;
using System;
using System.Diagnostics;

namespace Jacdac.NET.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncMain();
        }

        static async void AsyncMain()
        {
            Debug.WriteLine("connecting usb");
            NETPlatform.Init();
            var transport = new Jacdac.Transports.WebSockets.WebSocketTransport();
            transport.ConnectionChanged += (sender, newState) =>
            {
                Console.WriteLine($"usb: ${newState}");
            };

            var bus = new JDBus(transport);
            bus.DeviceConnected += (sender, conn) =>
            {
                var device = conn.Device;
                Console.WriteLine($"device connected ${device}");

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
