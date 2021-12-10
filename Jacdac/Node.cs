using System;

namespace Jacdac
{
    public delegate void NodeEventHandler(JDNode sender, EventArgs e);

    public sealed class PacketEventArgs : EventArgs
    {
        readonly Packet Packet;
        internal PacketEventArgs(Packet packet)
        {
            this.Packet = packet;
        }
    }
    public delegate void PacketEventHandler(JDNode sensor, PacketEventArgs e);

    public abstract class JDNode
    {
        internal JDNode()
        {

        }

        public event NodeEventHandler Changed;

        protected void RaiseChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    public abstract class JDServiceNode : JDNode
    {
        public readonly JDService Service;
        public readonly ushort Code;

        internal JDServiceNode(JDService service, ushort code)
        {
            this.Service = service;
            this.Code = code;
        }
    }
}
