#region namespaces
using Jacdac;
using Jacdac.Clients;
using System;
using System.Threading;
#endregion

namespace Jacdac.Samples
{
    internal class DoubleRotatingLights : ISample
    {
        public void Run(JDBus bus)
        {
            #region sources
            var leds = new LedPixelClient(bus, "leds1");
            var leds2 = new LedPixelClient(bus, "leds2");

            var red = 0xff0000;
            var purple = 0xff00ff;
            var blue = 0x0000ff;
            var green = 0x00ff00;
            var off = 0x000000;
            var paint = new LedPixelProgramBuilder()
                .Fade(off, green, off)
                .Show(0)
                .ToBuffer();
            var paint2 = new LedPixelProgramBuilder()
                .Fade(off, red, purple, blue, purple, red, off)
                .Show(0)
                .ToBuffer();
            var rotateBuilder = new LedPixelProgramBuilder();
            for (var i = 0; i < 10; ++i)
                rotateBuilder.Rotate(4).Show(20);
            var rotate = rotateBuilder.ToBuffer();
            rotateBuilder = null;

            leds.Connected += (s, e) =>
            {
                Console.WriteLine("leds connected...");
                leds.NumPixels = 300;
                leds.MaxPower = 2000;
                leds.Brightness = 0.1f;
                leds.Run(paint);
                leds.Run(paint);
                while (leds.IsConnected)
                {
                    leds.Run(rotate);
                    Thread.Sleep(200);
                }
            };
            leds.Disconnected += (s, e) => Console.WriteLine("leds disconnected...");

            leds2.Connected += (s, e) =>
            {
                Console.WriteLine("leds2 connected...");
                leds2.NumPixels = 300;
                leds2.MaxPower = 2000;
                leds2.Brightness = 0.1f;
                leds2.Run(paint2);
                leds2.Run(paint2);
                while (leds2.IsConnected)
                {
                    leds2.Run(rotate);
                    Thread.Sleep(200);
                }
            };
            leds2.Disconnected += (s, e) => Console.WriteLine("leds2 disconnected...");
            #endregion
        }
    }
}
