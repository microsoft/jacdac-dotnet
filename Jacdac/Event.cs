namespace Jacdac
{
    public sealed partial class JDEvent : JDServiceNode
    {
        private byte[] data;
        public uint Count = 0;

        internal JDEvent(JDService service, ushort code)
            : base(service, code)
        {

        }

        public byte[] Data
        {
            get
            {
                return this.data;
            }
        }

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
            var device = this.Service.Device;
            if (device?.Bus == null) return false;

            var ec = device.EventCounter + 1;
            // how many packets ahead and behind current are we?
            var ahead = (pkt.EventCounter - ec) & Jacdac.Constants.CMD_EVENT_COUNTER_MASK;
            var behind = (ec - pkt.EventCounter) & Jacdac.Constants.CMD_EVENT_COUNTER_MASK;
            // ahead == behind == 0 is the usual case, otherwise
            // behind < 60 means this is an old event (or retransmission of something we already processed)
            var old = behind < 60;
            var missed5 = ahead < 5;
            var isahead = ahead > 0;

            // ahead < 5 means we missed at most 5 events,
            // so we ignore this one and rely on retransmission
            // of the missed events, and then eventually the current event
            if (isahead && (old || missed5)) return false;

            this.data = pkt.Data;
            this.Count++;

            // update device counter
            device.EventCounter = pkt.EventCounter;
            this.RaiseChanged();

            return true;
        }
    }
}
