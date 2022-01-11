using Jacdac.Clients;
using System;
using System.Threading;

namespace Jacdac.Samples
{
    public sealed class HumidityTracker : ISample
    {
        public uint ProductIdentifier { get => 0x36ea1209; }
        public void Run(JDBus bus)
        {
            var humidity = new HumidityClient(bus, "humidity");
            // read humidity in a timer
            new Timer(state =>
            {
                try
                {
                    var h = humidity.Humidity;
                    Console.WriteLine($"humidity: {h}");
                }
                catch (ClientDisconnectedException) { }
            }, null, 0, 500);
        }
    }
}
