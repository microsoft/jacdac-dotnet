#region namespaces
using Jacdac;
using Jacdac.Clients;
using System;
using System.Threading;
#endregion

namespace Jacdac.Samples
{
    internal class TableLights : ISample
    {
        public void Run(JDBus bus)
        {
            #region sources
            var topLeds = new LedPixelClient(bus, "topLeds");
            //var bottomLeds = new LedPixelClient(bus, "bottomLeds");

            topLeds.Connected += (s, e) =>
            {
                Console.WriteLine("top leds connected...");
                topLeds.NumPixels = 300;
                topLeds.MaxPower = 2000;
                topLeds.Brightness = 0.2f;

                // paint color
                var red = 0xff0000;
                var blue = 0x0000ff;
                var paint = LedPixelEncoder.ToBuffer(
@"fade # # #
show 0", new object[] { red, blue, red });
                topLeds.Run(paint);

                // rotate
                var rotate = LedPixelEncoder.ToBuffer(
@"rotfwd 4
show 20"
);
                while (topLeds.IsConnected)
                {
                    topLeds.Run(rotate);
                    Thread.Sleep(50);
                }
            };

            var led = new LedClient(bus, "led");
            // wait for jacdac to find a LED service
            led.Connected += (s, e) =>
            {
                // as long as the led is connected, blink
                while (led.IsConnected)
                {
                    // send red 24-bit RGB color (same as HTML colors!)
                    led.SetColor(0xff0000);
                    Thread.Sleep(500);
                    // send blue
                    led.SetColor(0x0000ff);
                    Thread.Sleep(500);
                }
            };
            #endregion
        }
    }
}
