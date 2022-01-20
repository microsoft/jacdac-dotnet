using System;

namespace Jacdac
{
    public sealed partial class JDEvent : JDServiceNode
    {
        private byte[] data;
        private TimeSpan lastGetTimestamp = TimeSpan.MinValue;
        public uint Count { get; private set; } = 0;

        internal JDEvent(JDService service, ushort code)
            : base(service, code)
        {

        }

        public byte[] Data { get => this.data; }

        public TimeSpan LastGetTimestamp { get => this.lastGetTimestamp; }

        public object[] Values
        {
            get
            {
                // TODO: decode event data
                return PacketEncoding.Empty;
            }
        }
        public override string ToString()
        {
            return this.Service.ToString() + "{" + this.Code.ToString("x2") + "}";
        }

        internal bool ProcessPacket(Packet pkt)
        {
            if (pkt.IsRepeated) return false;

            this.Count++;
            this.RaiseChanged();
            // update device counter
            if (this.Code == (ushort)SystemEvent.StatusCodeChanged)
            {
                var reg = this.Service.GetRegister((ushort)SystemReg.StatusCode);
                if (reg != null)
                    reg.ReceiveData(this.data, this.lastGetTimestamp);
            }

            return true;
        }
    }
}
