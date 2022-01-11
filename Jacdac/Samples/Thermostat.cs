using Jacdac.Clients;
using System;
using System.Threading;

namespace Jacdac.Samples
{
    internal class Thermostat : ISample
    {
        public uint ProductIdentifier => 0x3ef747de;

        public void Run(JDBus bus)
        {
            var thermometer = new TemperatureClient(bus, "temp");
            var relay = new RelayClient(bus, "relay");
            var lastCommand = DateTime.MinValue;
            var desired = 68; // 68F
            thermometer.ReadingChanged += (s, e) =>
            {
                try
                {
                    var temp = thermometer.Temperature;
                    var error = temp - desired;
                    // turn on when temperature drops
                    if (error < -1)
                        relay.Active = true;
                    // turn off when too hot
                    else if (error > 0)
                        relay.Active = false;
                    // 0..1 zone, do nothing
                }
                catch (ClientDisconnectedException) { }
            };
        }
    }
}
