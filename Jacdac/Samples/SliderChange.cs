#region namespaces
using Jacdac;
using Jacdac.Clients;
using System;
#endregion

namespace Jacdac.Samples
{
    public sealed class SliderChange : ISample
    {
        public void Run(JDBus bus)
        {
            #region sources
            var slider = new PotentiometerClient(bus, "slider");
            slider.Connected += (s, e) => Console.WriteLine("connected");
            slider.Disconnected += (s, e) => Console.WriteLine("connected");
            slider.ReadingChanged += (s, e) =>
            {
                var position = slider.Position;
                Console.WriteLine($"position: {position}");
            };
            #endregion
        }
    }
}
