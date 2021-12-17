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
                (uint)(byte)((trgb >> 16) & 0xff),
                (uint)(byte)((trgb >> 8) & 0xff),
                (uint)(byte)(trgb & 0xff),
                (uint)(byte)((trgb >> 24) & 0xff)
            };
        }

        public void Blink(uint from, uint to, int interval, int repeat)
        {
            if (interval <= 0 || repeat <= 0) return;

            var aid = ++this.animationId;
            var on = PacketEncoding.Pack("u8 u8 u8 u8", trgbToValues(from));
            var off = PacketEncoding.Pack("u8 u8 u8 u8", trgbToValues(to));
            new Thread(() =>
            {
                for (var i = 0; i < repeat; ++i)
                {
                    var onPkt = Packet.FromCmd(this.Code, on);
                    this.Service.SendPacket(onPkt);
                    Thread.Sleep(interval);
                    if (this.animationId != aid) return;

                    var offPkt = Packet.FromCmd(this.Code, off);
                    this.Service.SendPacket(offPkt);
                    Thread.Sleep(interval);
                    if (this.animationId != aid) return;
                }
            }).Start();
        }
    }
}
