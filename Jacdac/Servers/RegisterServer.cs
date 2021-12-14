using System;

namespace Jacdac.Servers
{
    public abstract class JDRegisterServer : JDNode
    {
        public JDServer Server;
        public readonly ushort Code;
        public JDRegisterServer(ushort code)
        {
            this.Code = code;
        }
        public abstract void ProcessPacket(Packet pkt);
    }

    public delegate object[] RegisterGetHandler(JDRegisterServer server);

    public sealed class JDDynamicRegisterServer : JDRegisterServer
    {
        readonly string format;
        readonly RegisterGetHandler dataFactory;
        public JDDynamicRegisterServer(ushort code, string format, RegisterGetHandler dataFactory)
            : base(code)
        {
            this.format = format;
            this.dataFactory = dataFactory;
        }

        public override void ProcessPacket(Packet pkt)
        {
            if (pkt.IsRegisterGet)
            {
                var data = PacketEncoding.Pack(this.format, this.dataFactory(this));
                var server = this.Server;
                var resp = Packet.From((ushort)(Jacdac.Constants.CMD_GET_REG | this.Code), data);
                this.Server.SendPacket(resp);
            }
        }
    }

    public sealed class JDStaticRegisterServer : JDRegisterServer
    {
        public readonly string Format;
        public byte[] Data;
        public TimeSpan LastSetTime;

        public JDStaticRegisterServer(ushort code, string format, object[] value)
            : base(code)
        {
            this.Format = format;
            this.Data = new byte[0];
            this.LastSetTime = TimeSpan.Zero;
            this.SetValue(value);
        }

        public void SetValue(object[] value)
        {
            var packed = PacketEncoding.Pack(this.Format, value);
            if (!Util.BufferEquals(this.Data, packed))
            {
                this.Data = packed;
                this.RaiseChanged();
            }
        }

        public object[] GetValue()
        {
            if (this.Data.Length == 0) return new object[0];
            return PacketEncoding.UnPack(this.Format, this.Data);
        }

        public void SendGet()
        {
            var server = this.Server;
            if (server == null) return;

            var pkt = Packet.From((ushort)(Jacdac.Constants.CMD_GET_REG | this.Code), this.Data);
            this.Server.SendPacket(pkt);
        }

        public override void ProcessPacket(Packet pkt)
        {
            if (pkt.IsRegisterGet)
                this.SendGet();
            else if (pkt.IsRegisterSet)
                this.SetData(pkt);
        }

        private void SetData(Packet pkt)
        {
            try
            {
                this.LastSetTime = pkt.Timestamp;
                var values = PacketEncoding.UnPack(this.Format, pkt.Data);
                this.SetValue(values);
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("invalid format");
            }
        }
    }
}
