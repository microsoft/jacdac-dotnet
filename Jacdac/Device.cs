using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace Jacdac
{
    public sealed partial class JDDevice : JDBusNode
    {
        static uint Fnv1(byte[] data)
        {
            var h = 0x811c9dc5;
            for (var i = 0; i < data.Length; ++i)
            {
                h = (h * 0x1000193) ^ data[i];
            }
            return h;
        }
        static uint Hash(byte[] buf, int bits)
        {
            if (bits < 1) return 0;
            var h = Fnv1(buf);
            if (bits >= 32) return h >> 0;
            else return (uint)((h ^ (h >> bits)) & ((1 << bits) - 1)) >> 0;
        }
        public static string ShortDeviceId(string devid)
        {
            var h = Hash(HexEncoding.ToBuffer(devid), 30);
            return new String(new char[] {
                (char)(0x41 + (h % 26)) ,
                (char)(0x41 + ((h / 26) % 26)) ,
                (char)(0x30 + ((h / (26 * 26)) % 10)) ,
                (char)(0x30 + ((h / (26 * 26 * 10)) % 10))
                });
        }

        private JDBus bus;
        public readonly string DeviceId;
        public readonly string ShortId;
        public TimeSpan LastSeen;
        public uint EventCounter;

        byte[] _servicesData;
        JDService[] _services = JDService.EmptyServices;
        LEDController _statusLight = null;

        public JDDevice(JDBus bus, string deviceId)
        {
            this.bus = bus;
            this.DeviceId = deviceId;
            this.ShortId = ShortDeviceId(this.DeviceId);
            this.LastSeen = bus.Timestamp;
            this._servicesData = Packet.EmptyData;
        }
        public override JDBus Bus
        {
            get { return bus; }
        }

        public void Disconnect()
        {
            this.bus = null;
        }

        /**
         * Gets the control announce flag from the annouce packet.
         * @category Control
         */
        public ControlAnnounceFlags AnnounceFlags
        {
            get => (ControlAnnounceFlags)(this._servicesData != null ? BitConverter.ToUInt16(this._servicesData, 0) : 0);
        }

        public ControlAnnounceFlags StatusLightFlags
        {
            get => this.AnnounceFlags & ControlAnnounceFlags.StatusLightRgbFade;
        }

        public LEDController StatusLight
        {
            get
            {
                if (this._statusLight == null && this.StatusLightFlags != ControlAnnounceFlags.StatusLightNone)
                    this._statusLight = new LEDController(
                        this._services[0],
                        (ushort)Jacdac.ControlCmd.SetStatusLight
                    );
                return this._statusLight;
            }
        }

        public override string ToString()
        {
            return this.ShortId;
        }

        public void Reset()
        {
            var ctrl = this.GetService(0);
            if (ctrl != null)
                ctrl.SendPacket(Packet.FromCmd((ushort)ControlCmd.Reset));
        }

        public void Identify()
        {
            var statusLight = this.StatusLight;
            if (statusLight != null)
                statusLight.Blink(0x0000ff, 0, 262, 4);
            else
            {
                var ctrl = this.GetService(0);
                ctrl.SendPacket(Packet.FromCmd((ushort)ControlCmd.Identify));
            }
        }

        public void ProcessPacket(Packet pkt)
        {
            if (pkt.IsCrcAck)
                this.ReceiveAck(pkt);
            else
            {
                var srvs = this._services;
                var i = pkt.ServiceIndex;
                if (srvs != null && i < srvs.Length)
                    srvs[i].ProcessPacket(pkt);
            }
        }

        public void ProcessAnnouncement(Packet pkt)
        {
            var data = pkt.Data;
            var changed = false;
            uint w0 = this._servicesData.Length == 0
                ? 0
                : BitConverter.ToUInt32(this._servicesData, 0);
            uint w1 = data.Length == 0 ? 0 : BitConverter.ToUInt32(data, 0);
            var restarted = w1 != 0 &&
            (w1 & Jacdac.Constants.JD_ADVERTISEMENT_0_COUNTER_MASK) <
                (w0 & Jacdac.Constants.JD_ADVERTISEMENT_0_COUNTER_MASK);
            var servicesChanged = !Util.BufferEquals(pkt.Data, this._servicesData, 4);

            // compare service data
            if (servicesChanged)
            {
                this.InitServices(pkt.Data);
                this.Announced?.Invoke(this, EventArgs.Empty);
                changed = true;
            }


            // notify of any changes
            if (restarted)
                this.Restarted?.Invoke(this, EventArgs.Empty);
            if (changed || restarted)
                this.RaiseChanged();
        }

        private uint[] ServiceClasses
        {
            get
            {
                if (this._servicesData == null) return new uint[0];

                var data = this._servicesData;
                var n = data == null ? 0 : data.Length >> 2;
                var res = new uint[n];
                for (var i = 1; i < res.Length; ++i)
                {
                    res[i] = BitConverter.ToUInt32(data, i * 4);
                }
                return res;
            }
        }

        public bool HasService(uint serviceClass)
        {
            if (serviceClass == 0) return true;

            var data = this._servicesData;
            var n = data == null ? 0 : data.Length >> 2;
            for (var i = 1; i < n; ++i)
            {
                var sc = BitConverter.ToUInt32(data, i * 4);
                if (sc == serviceClass) return true;
            }
            return false;
        }

        private void InitServices(byte[] serviceData)
        {
            var services = this._services;
            foreach (var service in services)
                service.Device = null;
            this._services = JDService.EmptyServices;
            this._servicesData = serviceData;
            this._statusLight = null;
            var serviceClasses = this.ServiceClasses;
            if (serviceClasses.Length > 0)
            {
                var s = new JDService[serviceClasses.Length];
                for (byte i = 0; i < s.Length; ++i)
                {
                    s[i] = new JDService(this, i, serviceClasses[i]);
                }
                this._services = s;
            }
        }

        public JDService[] GetServices()
        {
            return this._services;
        }

        public JDService GetService(uint serviceIndex)
        {
            return serviceIndex < this._services.Length ? this._services[serviceIndex] : null;
        }

        sealed class Ack
        {
            public Packet Packet;
            public int RetriesLeft;
            public ManualResetEvent Event;
            public bool Error;

            public void Set()
            {
                var ev = this.Event;
                if (ev == null) return;

                this.Event = null;
                ev.Set();
            }
        }
        Ack[] _acks;

        private void RefreshAcks(Ack[] acks, int done)
        {
            if (acks.Length == done || this.Bus == null)
                this._acks = null;
            else
            {
                var newAcks = new Ack[acks.Length - done];
                var k = 0;
                for (var i = 0; i < acks.Length; ++i)
                {
                    var ack = acks[i];
                    if (ack.Packet != null) newAcks[k] = ack;
                }
                System.Diagnostics.Debug.Assert(k == newAcks.Length);
                this._acks = newAcks;
            }
        }

        private void SignalAcks(Ack[] acks)
        {
            foreach (var ack in acks)
            {
                if (ack.Packet == null)
                    ack.Set();
            }
        }

        private void ReceiveAck(Packet ackPkt)
        {
            this.Debug($"{this}: receive ack {ackPkt.Crc}");
            var serviceCommand = ackPkt.ServiceCommand;
            Ack[] acks;
            var received = 0;
            lock (this)
            {
                acks = this._acks == null ? new Ack[0] : (Ack[])this._acks.Clone();
                foreach (var ack in acks)
                {
                    var pkt = ack.Packet;
                    if (pkt != null && pkt.Crc == serviceCommand)
                    {
                        ack.Packet = null;
                        received++;
                    }
                }
                if (received > 0)
                {
                    this.Debug($"{this}: ack received {received}");
                    this.RefreshAcks(acks, received);
                }
            }
            this.SignalAcks(acks);
        }

        private void ResendAcks()
        {
            Ack[] acks;
            var errors = 0;
            lock (this)
            {
                acks = this._acks == null ? new Ack[0] : (Ack[])this._acks.Clone();
                //this.Debug($"{this}: resend acks {acks.Length}");
                foreach (var ack in acks)
                {
                    if (ack.Packet == null) continue; // already processed
                    if (--ack.RetriesLeft < 0)
                    {
                        this.Debug($"{this}: ack error {ack.Packet}");
                        ack.Packet = null;
                        ack.Error = true;
                        ack.Event.Set();
                        errors++;
                    }
                    else
                    {
                        if (this.Bus == null || this.Bus.IsPassive) continue; // disconnected
                        this.Bus.SelfDeviceServer.SendPacket(ack.Packet);
                    }
                }
                // filter out error packets
                if (errors > 0)
                    this.RefreshAcks(acks, errors);


                // schedule acks again
                if (this._acks != null)
                    new Timer(state => this.ResendAcks(), null, 40, Timeout.Infinite);
            }

            // trigger errors
            if (errors > 0)
                this.SignalAcks(acks);
        }

        private void WaitForAck(Packet pkt)
        {
            Stats.WaitForAcks++;
            var ack = new Ack
            {
                Packet = pkt,
                RetriesLeft = 4,
                Event = new ManualResetEvent(false)
            };
            lock (this)
            {
                if (this._acks == null)
                {
                    this._acks = new Ack[1] { ack };
                    new Timer((state) => this.ResendAcks(), null, 40, Timeout.Infinite);
                }
                else
                {
                    var newAcks = new Ack[this._acks.Length];
                    this._acks.CopyTo(newAcks, 0);
                    newAcks[newAcks.Length - 1] = ack;
                    this._acks = newAcks;
                }
            }
            ack.Event.WaitOne();
            if (ack.Error)
                throw new AckException(pkt);
        }

        public void SendPacket(Packet pkt)
        {
            if (this.Bus == null || this.Bus.IsPassive) return;

            System.Diagnostics.Debug.Assert(!pkt.IsMultiCommand);
            pkt.DeviceId = this.DeviceId;
            this.Bus.SelfDeviceServer.SendPacket(pkt);
            if (pkt.RequiresAck)
                this.WaitForAck(pkt);
        }

        public bool IsUniqueBrain
        {
            get { return this.HasService(Jacdac.ServiceClasses.UniqueBrain); }
        }
        public bool IsBridge
        {
            get { return this.HasService(Jacdac.ServiceClasses.Bridge); }
        }

        public bool IsProxy
        {
            get { return this.HasService(Jacdac.ServiceClasses.Proxy); }
        }

        public bool IsDashboard
        {
            get { return this.HasService(Jacdac.ServiceClasses.Dashboard); }
        }

        public event NodeEventHandler Restarted;

        public event NodeEventHandler Announced;

        public sealed class Comparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var l = (JDDevice)x;
                var r = (JDDevice)y;
                return l.DeviceId.CompareTo(r.DeviceId);
            }
        }
    }

    public sealed class DeviceEventArgs : EventArgs
    {
        public readonly JDDevice Device;
        internal DeviceEventArgs(JDDevice device)
        {
            this.Device = device;
        }
    }
    public delegate void DeviceEventHandler(JDNode node, DeviceEventArgs e);
}