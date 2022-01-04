﻿using Jacdac.Transports;
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
            foreach (var arg in args)
            {
                if (arg == "devtools")
                {
                    Console.WriteLine("adding devtools connection");
                    bus.AddTransport(new WebSocketTransport());
                }
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
