namespace Jacdac
{
    public sealed class JDEvent : JDServiceNode
    {
        internal JDEvent(JDService service, ushort code)
            : base(service, code)
        {

        }

        public override string ToString()
        {
            return this.Service.ToString() + "{" + this.Code.ToString("x2") + "}";
        }

        internal void ProcessPacket(Packet pkt)
        {

        }
    }
}
