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
        public bool Stream = false;

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
            var pkt = Packet.FromCmd(cmd, data);
            if (ack)
                pkt.RequiresAck = true;
            this.LastSetTimestamp = this.Service.Device.Bus.Timestamp;
            this.Service.SendPacket(pkt);
        }

        public void SendGet(bool ack = false)
        {
            if (this.NotImplemented) return;

            this.LastGetAttempts++;
            ushort cmd = (ushort)(Jacdac.Constants.CMD_GET_REG | this.Code);
            var pkt = Packet.FromCmd(cmd);
            if (ack)
                pkt.RequiresAck = true;
            this.Service.SendPacket(pkt);
        }

        public void RefreshMaybe()
        {
            if (this.NotImplemented) return;

            var now = this.Service.Device.Bus.Timestamp;
            var age = (now - this.LastGetTimestamp).TotalMilliseconds;
            var noDataYet = this.Data == null;
            var backoff = this.LastGetAttempts;
            var service = this.Service;
            var hasListeners = this.HasChangedListeners();

            if (!this.NeedsRefresh && !hasListeners)
                return;

            if (this.Code == (ushort)Jacdac.SystemReg.Reading)
            {
                var interval = this.ResolveStreamingInterval();
                var samplesAge = this.ResolveStreamingSamplesAge();
                var midAge = (interval * 0xff) >> 1;
                if (samplesAge.TotalMilliseconds > midAge)
                    this.Service.GetRegister((ushort)Jacdac.SensorReg.StreamingSamples).SendSet(PacketEncoding.Pack("u8", new object[] { (uint)0xff }));
                if (this.NeedsRefresh || (noDataYet && age > 1000))
                    this.SendGet();
            }
            else
            {
                var expiration = Math.Min(
                          Jacdac.Constants.REGISTER_POLL_REPORT_MAX_INTERVAL,
                          (noDataYet
                              ? Jacdac.Constants.REGISTER_POLL_FIRST_REPORT_INTERVAL
                              : Jacdac.Constants.REGISTER_POLL_REPORT_INTERVAL) *
                              (1 << backoff)
                      );
                if (this.NeedsRefresh || age > expiration)
                    this.SendGet();
            }
        }

        private TimeSpan ResolveStreamingSamplesAge()
        {
            var samplesReg = this.Service.GetRegister((ushort)Jacdac.SensorReg.StreamingSamples, true);
            if (samplesReg.Data == null)
                return TimeSpan.MaxValue;

            return this.Service.Device.Bus.Timestamp - samplesReg.LastSetTimestamp;
        }

        private uint ResolveStreamingInterval()
        {
            var intervalReg = this.Service.GetRegister((ushort)Jacdac.SensorReg.StreamingInterval, true);
            if (intervalReg.Data != null)
                return (uint)PacketEncoding.UnPack("u32", intervalReg.Data)[0];

            var preferredIntervalReg = this.Service.GetRegister((ushort)Jacdac.SensorReg.StreamingPreferredInterval, true);
            if (preferredIntervalReg.Data != null)
                return (uint)PacketEncoding.UnPack("u32", preferredIntervalReg.Data)[0];

            return Jacdac.Constants.REGISTER_POLL_STREAMING_INTERVAL;
        }
    }
}
