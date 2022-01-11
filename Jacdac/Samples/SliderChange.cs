using Jacdac.Clients;
using System;
using System.Threading;

namespace Jacdac.Samples
{
    public sealed class SliderChange : ISample
    {
        public uint ProductIdentifier { get => 0x3e71bd83; }
        public void Run(JDBus bus)
        {
            bus.DeviceConnected += (s, e) => Console.WriteLine($"device connected: {e.Device}");
            bus.DeviceDisconnected += (s, e) => Console.WriteLine($"device disconnected: {e.Device}");

            var slider = new PotentiometerClient(bus, "slider");
            slider.Connected += (s, e) => Console.WriteLine("connected");
            slider.Disconnected += (s, e) => Console.WriteLine("connected");
            slider.ReadingChanged += (s, e) =>
            {
                var position = slider.Position;
                Console.WriteLine($"position: {position}");
            };
        }
    }
}
