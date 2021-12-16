using System;

namespace Jacdac
{
    public sealed class JDRegister : JDServiceNode
    {
        private bool notImplemented = false;
        public TimeSpan LastGetTimestamp = TimeSpan.Zero;
        public TimeSpan LastSetTimestamp = TimeSpan.Zero;
        public bool NeedsRefresh = false;
        Packet _lastReportPkt;
        public int LastGetAttempts = 0;

        internal JDRegister(JDService service, ushort code)
            : base(service, code)
        {
        }

        public byte[] Data
        {
            get
            {
                if (this._lastReportPkt != null)
                    return this._lastReportPkt.Data;
                return null;
            }
        }

        public bool NotImplemented
        {
            get { return this.notImplemented; }
            set
            {
                if (value != this.notImplemented)
                {
                    this.notImplemented = value;
                    this.RaiseChanged();
                }
            }
        }

        public override string ToString()
        {
            return $"{this.Service.ToString()}[{this.Code.ToString("x2")}]";
        }

        internal void ProcessPacket(Packet pkt)
        {
            if (pkt.IsRegisterGet)
            {
                this.ProcessReport(pkt);
            }
            else if (pkt.IsRegisterSet)
            {
                // another device sent a set packet to this register
                // so most likely it's value changed
                // clear any data caching to force updating the value
                this.LastGetTimestamp = TimeSpan.Zero;
            }
        }

        private void ProcessReport(Packet pkt)
        {
            var updated = this.NeedsRefresh || !Util.BufferEquals(this.Data, pkt.Data);
            this._lastReportPkt = pkt;
            this.LastGetAttempts = 0; // reset counter
            this.LastGetTimestamp = pkt.Timestamp;
            this.NeedsRefresh = false;

            //this.emit(REPORT_RECEIVE, this)
            if (updated)
            {
                //this.emitPropagated(REPORT_UPDATE, this)                
                this.RaiseChanged();
            }
        }

        public void SendSet(byte[] data, bool ack = false)
        {
            if (this.NotImplemented) return;

            ushort cmd = (ushort)(Jacdac.Constants.CMD_SET_REG | this.Code);
            var pkt = Packet.From(cmd, data);
            if (ack)
                pkt.RequiresAck = true;
            this.LastSetTimestamp = this.Service.Device.Bus.Timestamp;
            this.Service.SendPacket(pkt);
        }

        public void SendGet()
        {
            if (this.NotImplemented) return;

            this.LastGetAttempts++;
            ushort cmd = (ushort)(Jacdac.Constants.CMD_GET_REG | this.Code);
            var pkt = Packet.OnlyHeader(cmd);
            this.Service.SendPacket(pkt);
        }
    }
}
