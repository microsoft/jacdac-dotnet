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
            var paint = LedPixelEncoding.ToBuffer(
@"fade # # # # # # #
show 0", new object[] { off, red, purple, blue, purple, red, off });

            var rotate = LedPixelEncoding.ToBuffer(
@"
rotfwd 2
show 20
rotfwd 2
show 20
rotfwd 2
show 20
rotfwd 2
show 20
rotfwd 2
show 20
"
);
            leds.Connected += (s, e) =>
            {
                Console.WriteLine("leds connected...");
                leds.NumPixels = 300;
                leds.MaxPower = 2000;
                leds.Brightness = 0.1f;
                leds.Run(paint);
                while (leds.IsConnected)
                {
                    Console.Write(".");
                    leds.Run(rotate);
                    Thread.Sleep(100);
                }
            };
            leds.Disconnected += (s, e) => Console.WriteLine("leds disconnected...");
            #endregion
        }
    }
}
