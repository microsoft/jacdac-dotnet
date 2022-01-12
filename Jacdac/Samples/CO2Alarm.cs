#region namespaces
using Jacdac;
using Jacdac.Clients;
using System;
#endregion

namespace Jacdac.Samples
{
    internal class CO2Alarm : ISample
    {
        public void Run(JDBus bus)
        {
            // DISCLAIMER: This is a sample and should not be used as reference to build
            // any kind of device.
            #region sources
            var co2 = new ECO2Client(bus, "co2");
            var buzzer = new BuzzerClient(bus, "buzzer");
            var led = new LedClient(bus, "led");
            co2.ReadingChanged += (s, e) =>
            {
                var value = co2.ECO2;
                // too much CO2
                if (value > 1000)
                {
                    Console.WriteLine("too much CO2, ventilate!");
                    // turn lights on
                    led.SetColor(0xff0000);
                    // sound alarm
                    buzzer.PlayNote(800, 1, 500);
                }
                else if (value < 900)
                {
                    Console.WriteLine("CO2 ok");
                    // turn off light by showing black
                    led.SetColor(0);
                }
            };
            #endregion
        }
    }
}
