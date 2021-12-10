using System;

namespace Jacdac
{
    public sealed class JDRegister : JDServiceNode
    {
        public bool NotImplemented = false;
        public TimeSpan LastGetTimestamp = TimeSpan.Zero;
        Packet _lastReportPkt;

        internal JDRegister(JDService service, ushort code)
            : base(service, code)
        {
        }

        public override string ToString()
        {
            return this.Service.ToString() + "[" + this.Code.ToString("x2") + "]";
        }

        internal void ProcessPacket(Packet pkt)
        {

        }
    }
}
