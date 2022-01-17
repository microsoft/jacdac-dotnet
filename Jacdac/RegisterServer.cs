using System;

namespace Jacdac
{
    public abstract class JDRegisterServer : JDNode
    {
        public JDServiceServer Service;
        public readonly ushort Code;
        protected JDRegisterServer(ushort code)
        {
            this.Code = code;
        }

        public override JDBus Bus => this.Service?.Bus;

        public abstract bool ProcessPacket(Packet pkt);
    }

    public delegate object[] RegisterGetHandler(JDRegisterServer server);
    public delegate void RegisterSetHandler(JDRegisterServer server, object[] values);

    public sealed class JDDynamicRegisterServer : JDRegisterServer
    {
        readonly string format;
        readonly RegisterGetHandler dataGetter;
        readonly RegisterSetHandler dataSetter;
        public JDDynamicRegisterServer(ushort code, string format, RegisterGetHandler dataGetter, RegisterSetHandler dataSetter = null)
            : base(code)
        {
            this.format = format;
            this.dataGetter = dataGetter;
            this.dataSetter = dataSetter;
        }

        public override bool ProcessPacket(Packet pkt)
        {
            if (pkt.IsRegisterGet)
            {
                var values = this.dataGetter(this);
                if (values == null)
                    return false;

                var data = PacketEncoding.Pack(this.format, values);
                var server = this.Service;
                var resp = Packet.From((ushort)(Jacdac.Constants.CMD_GET_REG | this.Code), data);
                server.SendPacket(resp);
                return true;
            }
            else if (pkt.IsRegisterSet && this.dataSetter != null)
            {
                var data = pkt.Data;
                var server = this.Service;
                var values = PacketEncoding.UnPack(this.format, data);
                this.dataSetter(this, values);
                return true;
            }

            return false;
        }
    }

    public sealed class JDStaticRegisterServer : JDRegisterServer
    {
        public readonly string Format;
        public byte[] Data;
        public TimeSpan LastSetTime;
        public bool IsConst = false;

        public JDStaticRegisterServer(ushort code, string format, object[] values)
            : base(code)
        {
            this.Format = format;
            this.Data = Packet.EmptyData;
            this.LastSetTime = TimeSpan.Zero;
            this.SetValues(values);
        }

        public void SetValues(object[] value)
        {
            var packed = PacketEncoding.Pack(this.Format, value);
            if (!Util.BufferEquals(this.Data, packed))
            {
                this.Data = packed;
                this.RaiseChanged();
            }
        }

        public object[] GetValues()
        {
            if (this.Data.Length == 0) return new object[0];
            return PacketEncoding.UnPack(this.Format, this.Data);
        }

        public bool GetValueAsBool()
        {
            var values = this.GetValues();
            if (values == null || values.Length == 0) return false;
            var b = Util.UnboxInt(values[0]) != 0;
            return b;
        }

        public string GetValueAsString()
        {
            var values = this.GetValues();
            if (values == null || values.Length == 0) return null;
            var b = (string)values[0];
            return b;
        }

        public void SendGet()
        {
            var server = this.Service;
            if (server == null) return;

            var pkt = Packet.From((ushort)(Jacdac.Constants.CMD_GET_REG | this.Code), this.Data);
            this.Service.SendPacket(pkt);
        }

        public override bool ProcessPacket(Packet pkt)
        {
            if (pkt.IsRegisterGet)
            {
                this.SendGet();
                return true;
            }
            else if (pkt.IsRegisterSet && !this.IsConst)
            {
                this.SetData(pkt);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetData(Packet pkt)
        {
            try
            {
                this.LastSetTime = pkt.Timestamp;
                var values = PacketEncoding.UnPack(this.Format, pkt.Data);
                this.SetValues(values);
            }
            catch (Exception)
            {
                this.LogDebug("invalid data format");
            }
        }
    }
}
