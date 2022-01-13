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

        public string Name
        {
            get
            {
                return this.ResolveName();
            }
        }

        private string ResolveName()
        {
            var spec = this.Specification;
            if (spec != null)
                return spec.name;

            // Reflection of Enum values not support in TinyCLR.

            return $"0x{ this.Code.ToString("x2")}";
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
            var name = this.Name;
            return $"{this.Service.ToString()}[{name}]";
        }

        public string GetHumanValue()
        {
            var data = this.Data;
            if (data == null) return "?";
            if (this.PackFormat == null)
                return HexEncoding.ToString(data);
            var values = PacketEncoding.UnPack(this.PackFormat, data);
            if (values.Length == 0)
                return "--";
            else if (values.Length == 1)
            {
                var value = values[0];
                if (this.Service.ServiceClass == ServiceClasses.Control && this.Code == (ushort)ControlReg.Uptime)
                    return new TimeSpan((long)((ulong)value * 10)).ToString();

                var str = value.ToString();
                if (this.PackFormat == "u32")
                    str += " (0x" + ((uint)value).ToString("x8") + ")";
                else if (this.PackFormat == "u16")
                    str += " (0x" + ((uint)value).ToString("x4") + ")";
                else if (this.PackFormat == "u8")
                    str += " (0x" + ((uint)value).ToString("x2") + ")";
                return str;
            }
            else
                return $"[{values.Length}";
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
            this.ReceiveData(pkt.Data, pkt.Timestamp);
        }

        internal void ReceiveData(byte[] data, TimeSpan timestamp)
        {
            var updated = this.NeedsRefresh || !Util.BufferEquals(this.Data, data);
            if (updated)
                this.Data = data;
            this.LastGetAttempts = 0; // reset counter
            this.LastGetTimestamp = timestamp;
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
            this.NeedsRefresh = true;
        }

        public void SendGet(bool ack = false)
        {
            var bus = this.Bus;
            if (this.NotImplemented || bus == null || bus.SelfDeviceServer.IsProxy) return;

            this.LastGetAttempts++;
            if (this.LastGetAttempts > 5)
                this.NeedsRefresh = false; // give up
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

        public void SendValues(object[] values, bool ack = false)
        {
            var packf = this.PackFormat;
            if (packf == null)
                throw new InvalidOperationException("register format unknown");
            var data = PacketEncoding.Pack(packf, values);
            if (!Util.BufferEquals(data, this.Data))
                this.SendSet(data, ack);
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
