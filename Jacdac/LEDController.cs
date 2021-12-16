using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Jacdac
{
    public sealed class LEDController
    {
        public readonly JDService Service;
        public readonly ushort Code;
        private uint animationId = 0;

        public LEDController(JDService service, ushort code)
        {
            this.Service = service;
            this.Code = code;
        }

        static object[] trgbToValues(uint trgb)
        {
            return new object[] {
                (byte)((trgb >> 16) & 0xff),
                (byte)((trgb >> 8) & 0xff),
                (byte)(trgb & 0xff),
                (byte)((trgb >> 24) & 0xff)
            };
        }

        public void Blink(uint from, uint to, int interval, int repeat)
        {
            var aid = ++this.animationId;
            var on = PacketEncoding.Pack("u8 u8 u8 u8", trgbToValues(from));
            var off = PacketEncoding.Pack("u8 u8 u8 u8", trgbToValues(to));
            new Thread(() =>
            {
                for (var i = 0; i < repeat; ++i)
                {
                    Debug.WriteLine("blink on");
                    var onPkt = Packet.From(this.Code, on);
                    this.Service.SendPacket(onPkt);
                    Thread.Sleep(interval);
                    if (this.animationId != aid) return;

                    Debug.WriteLine("blink off");
                    var offPkt = Packet.From(this.Code, off);
                    this.Service.SendPacket(offPkt);
                    Thread.Sleep(interval);
                    if (this.animationId != aid) return;
                }
            }).Start();
        }
    }
}
