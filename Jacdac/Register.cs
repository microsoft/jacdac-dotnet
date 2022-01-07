using System;
using System.Threading;

namespace Jacdac
{
    public sealed class JDRegister : JDServiceNode
    {
        private bool notImplemented = false;
        public TimeSpan LastGetTimestamp = TimeSpan.Zero;
        public TimeSpan LastSetTimestamp = TimeSpan.Zero;
        public bool NeedsRefresh = false;
        public byte[] Data;
        public int LastGetAttempts = 0;
        public bool IsStreaming = false;
        private string _packFormat;

        internal JDRegister(JDService service, ushort code)
            : base(service, code)
        {
        }

        public bool NotImplemented
        {
            get { return this.notImplemented; }
            set
            {
                if (value != this.notImplemented)
                    this.notImplemented = value;
            }
        }

        public override string ToString()
        {
            var spec = this.Specification;
            var descr = spec == null ? $"0x{this.Code.ToString("x2")}" : spec.name;
            return $"{this.Service.ToString()}[{descr}]";
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
            if (updated)
                this.Data = pkt.Data;
            this.LastGetAttempts = 0; // reset counter
            this.LastGetTimestamp = pkt.Timestamp;
            this.NeedsRefresh = false;

            this.ReportReceived?.Invoke(this, EventArgs.Empty);
            if (updated)
                this.RaiseChanged();
        }

        /**
         * Raised when a GET report is received
         */
        public event NodeEventHandler ReportReceived;

        public void SendSet(byte[] data, bool ack = false)
        {
            var bus = this.Bus;
            if (this.NotImplemented || bus == null) return;

            ushort cmd = (ushort)(Jacdac.Constants.CMD_SET_REG | this.Code);
            var pkt = Packet.FromCmd(cmd, data);
            if (ack)
                pkt.RequiresAck = true;
            this.LastSetTimestamp = bus.Timestamp;
            this.Service.SendPacket(pkt);
        }

        public void SendGet(bool ack = false)
        {
            var bus = this.Bus;
            if (this.NotImplemented || bus == null) return;

            this.LastGetAttempts++;
            ushort cmd = (ushort)(Jacdac.Constants.CMD_GET_REG | this.Code);
            var pkt = Packet.FromCmd(cmd);
            if (ack)
                pkt.RequiresAck = true;
            this.Service.SendPacket(pkt);
        }

        public void RefreshMaybe()
        {
            var bus = this.Bus;
            if (this.NotImplemented || bus == null) return;

            var now = bus.Timestamp;
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
                    this.Service.GetRegister((ushort)Jacdac.SensorReg.StreamingSamples).SendSet(PacketEncoding.Pack((byte)0xff));
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
            var bus = this.Bus;
            if (bus == null) return TimeSpan.MaxValue;

            var samplesReg = this.Service.GetRegister((ushort)Jacdac.SensorReg.StreamingSamples);
            if (samplesReg.Data == null)
                return TimeSpan.MaxValue;

            return bus.Timestamp - samplesReg.LastSetTimestamp;
        }

        private uint ResolveStreamingInterval()
        {
            var intervalReg = this.Service.GetRegister((ushort)Jacdac.SensorReg.StreamingInterval);
            if (intervalReg.Data != null)
                return BitConverter.ToUInt32(intervalReg.Data, 0);

            var preferredIntervalReg = this.Service.GetRegister((ushort)Jacdac.SensorReg.StreamingPreferredInterval);
            if (preferredIntervalReg.Data != null)
                return BitConverter.ToUInt32(preferredIntervalReg.Data, 0);

            return Jacdac.Constants.REGISTER_POLL_STREAMING_INTERVAL;
        }

        public ServiceRegisterSpec Specification
        {
            get
            {
                // find service spec
                var serviceSpec = this.Service.Specification;
                if (serviceSpec == null)
                    return null;
                // find register spec
                var registerSpecs = serviceSpec.registers;
                for (var i = 0; i < registerSpecs.Length; ++i)
                {
                    if (registerSpecs[i].code == this.Code)
                        return registerSpecs[i];
                }
                return null;
            }
        }

        public string PackFormat
        {
            get
            {
                if (this._packFormat == null)
                {
                    var spec = this.Specification;
                    if (spec != null)
                        this._packFormat = spec.packf;
                }
                return this._packFormat;
            }
            set
            {
                if (this._packFormat != value)
                {
                    if (this._packFormat != null && this._packFormat != value)
                        throw new InvalidOperationException("attempting to change pack format");
                    this._packFormat = value;
                }
            }
        }

        public object[] Values
        {
            get
            {
                var data = this.Data;
                if (data == null)
                    return PacketEncoding.Empty;

                var packf = this.PackFormat;
                if (packf == null)
                    return PacketEncoding.Empty;
                // deserialize
                var values = PacketEncoding.UnPack(packf, data);
                return values;
            }
        }

        public void SendValues(object[] values)
        {
            var packf = this.PackFormat;
            if (packf == null)
                throw new InvalidOperationException("register format unknown");
            this.Data = PacketEncoding.Pack(packf, values);
        }

        public object[] WaitForValues(int timeout = 1000)
        {
            var values = this.Values;
            if (values.Length > 0) return values;

            NodeEventHandler signal = null;
            try
            {
                var wait = new AutoResetEvent(false);
                signal = (JDNode node, EventArgs pkt) =>
                {
                    values = this.Values;
                    if (values.Length == 0)
                        // keep waiting
                        return;
                    this.ReportReceived -= signal;
                    wait.Set();
                };
                this.ReportReceived += signal;
                this.RefreshMaybe();
                wait.WaitOne(timeout, true);

                return values;
            }
            catch (Exception)
            {
                throw new ClientDisconnectedException();
            }
            finally
            {
                if (signal != null)
                    this.ReportReceived -= signal;
            }
        }
    }
}
