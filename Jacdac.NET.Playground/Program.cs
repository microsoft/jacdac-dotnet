using Jacdac.Clients;
using Jacdac.Samples;
using Jacdac.Transports;
using Jacdac.Transports.Spi;
using Jacdac.Transports.Usb;
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
                ProductIdentifier = sample.ProductIdentifier,
                SpecificationCatalog = new ServiceSpecificationCatalog()
            });
            bus.DeviceConnected += (s, e) => Console.WriteLine($"device connected: {e.Device}");
            bus.DeviceDisconnected += (s, e) => Console.WriteLine($"device disconnected: {e.Device}");

            // add transports
            foreach (var arg in args)
                switch (arg)
                {
                    case "spi":
                        Console.WriteLine("adding spi connection");
                        bus.AddTransport(SpiTransport.Create());
                        break;
                    case "usb":
                        Console.WriteLine("adding usb connection");
                        bus.AddTransport(UsbTransport.Create());
                        break;
                    case "devtools":
                        Console.WriteLine("adding devtools connection");
                        bus.AddTransport(WebSocketTransport.Create());
                        break;
                }

            //  run test
            new Thread(state => sample.Run(bus)).Start();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
