using Jacdac.Clients;
using System;
using System.Threading;

namespace Jacdac.Samples
{
    public sealed class SliderChange : ISample
    {
        public void Run(JDBus bus)
        {
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
