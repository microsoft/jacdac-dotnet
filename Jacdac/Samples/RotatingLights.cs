#region namespaces
using Jacdac;
using Jacdac.Clients;
using System;
using System.Threading;
#endregion

namespace Jacdac.Samples
{
    internal class RotatingLights : ISample
    {
        public void Run(JDBus bus)
        {
            #region sources
            var leds = new LedPixelClient(bus, "leds");

            var red = 0xff0000;
            var purple = 0xff00ff;
            var blue = 0x0000ff;
            var off = 0x000000;
            var paint = new LedPixelProgramBuilder()
                .Fade(off, red, purple, blue, purple, red, off)
                .Show(0)
                .ToBuffer();
            var rotateBuilder = new LedPixelProgramBuilder();
            for (var i = 0; i < 10; ++i)
                rotateBuilder.Rotate(10).Show(20);
            var rotate = rotateBuilder.ToBuffer();
            rotateBuilder = null;
            leds.Connected += (s, e) =>
            {
                Console.WriteLine("leds connected...");
                leds.NumPixels = 300;
                leds.MaxPower = 2000;
                leds.Brightness = 0.1f;
                leds.Run(paint);
                while (leds.IsConnected)
                {
                    leds.Run(rotate);
                    Thread.Sleep(200);
                }
            };
            leds.Disconnected += (s, e) => Console.WriteLine("leds disconnected...");
            #endregion
        }
    }
}
