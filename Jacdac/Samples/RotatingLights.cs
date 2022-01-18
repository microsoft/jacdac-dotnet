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
            // default configuration of the service
            leds.NumPixels = 300;
            leds.MaxPower = 2000;
            leds.Brightness = 0.1f;

            // led painting instructions
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

            // run once when a device connects or restarts
            leds.Configure += (s, e) =>
            {
                leds.Run(paint);
            };
            // runs when the device gets connected for the first time on the bus
            leds.Connected += (s, e) =>
            {
                // keep rotating leds until the strip disconnects
                while (leds.IsConnected)
                {
                    leds.Run(rotate);
                    Thread.Sleep(200);
                }
            };
            #endregion
        }
    }
}
