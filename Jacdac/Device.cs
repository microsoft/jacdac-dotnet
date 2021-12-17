using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace Jacdac
{
    public sealed class JDDevice : JDNode
    {
        private JDBus bus;
        public readonly string DeviceId;
        public TimeSpan LastSeen;
        public uint EventCounter;

        byte[] _servicesData;
        JDService[] _services = null;
        LEDController _statusLight = null;

        public JDDevice(JDBus bus, string deviceId)
        {
            this.bus = bus;
            this.DeviceId = deviceId;
            this.LastSeen = bus.Timestamp;
            this._servicesData = new byte[0];
        }

        public JDBus Bus
        {
            get { return bus; }
        }

        public void Disconnect()
        {
            this.bus = null;
        }

        public string ShortId
        {
            get { return Util.ShortDeviceId(this.DeviceId); }
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
                ctrl.SendPacket(Packet.FromCmd((ushort)Jacdac.ControlCmd.Reset));
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

            // compare service data
            var servicesChanged = !Util.BufferEquals(pkt.Data, this._servicesData, 4);
            if (servicesChanged)
                this._servicesData = pkt.Data;

            // check for restart
            if (
            w1 != 0 &&
            (w1 & Jacdac.Constants.JD_ADVERTISEMENT_0_COUNTER_MASK) <
                (w0 & Jacdac.Constants.JD_ADVERTISEMENT_0_COUNTER_MASK)
        )
            {
                this.InitServices(true);
                this.Restarted?.Invoke(this, EventArgs.Empty);
                changed = true;
            }

            // notify that services got updated
            if (servicesChanged)
            {
                if (!changed) this.InitServices(true);
                this.Announced?.Invoke(this, EventArgs.Empty);
                changed = true;
            }

            // notify that we've received an announce packet
            //this.bus.emit(DEVICE_PACKET_ANNOUNCE, this)
            //this.emit(PACKET_ANNOUNCE)

            // notify of any changes
            if (changed)
                this.RaiseChanged();
        }

        private uint[] ServiceClasses
        {
            get
            {
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

        private void InitServices(bool force)
        {
            if (force)
            {
                if (this._services != null)
                {
                    for (var i = 0; i < this._services.Length; i++)
                        this._services[i].Device = null;
                }
                this._services = null;
            }

            if (null == this._services && null != this._servicesData)
            {
                var serviceClasses = this.ServiceClasses;
                var s = new JDService[serviceClasses.Length];
                for (byte i = 0; i < s.Length; ++i)
                {
                    s[i] = new JDService(this, i, serviceClasses[i]);
                }
                this._services = s;
                //this.lastServiceUpdate = this.bus.timestamp
                this.RaiseChanged();
            }
        }

        public JDService[] GetServices()
        {
            var srvs = this._services == null ? new JDService[0] : (JDService[])this._services.Clone();
            return srvs;
        }

        public JDService GetService(byte serviceIndex)
        {
            return this._services != null && serviceIndex < this._services.Length ? this._services[serviceIndex] : null;
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
                Debug.Assert(k == newAcks.Length);
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
            Debug.WriteLine($"{this}: receive ack {ackPkt.Crc}");
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
                    Debug.WriteLine($"{this}: ack received {received}");
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
                Debug.WriteLine($"{this}: resend acks {acks.Length}");
                foreach (var ack in acks)
                {
                    if (ack.Packet == null) continue; // already processed
                    if (--ack.RetriesLeft < 0)
                    {
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
                {
                    Debug.WriteLine($"{this}: ack errors {errors}");
                    this.RefreshAcks(acks, errors);
                }


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

            Debug.Assert(!pkt.IsMultiCommand);
            pkt.DeviceId = this.DeviceId;
            this.Bus.SelfDeviceServer.SendPacket(pkt);
            if (pkt.RequiresAck)
                this.WaitForAck(pkt);
        }

        public bool IsUniqueBrain
        {
            get { return this.HasService(Jacdac.UniqueBrainConstants.ServiceClass); }
        }
        public bool IsBridge
        {
            get { return this.HasService(Jacdac.BridgeConstants.ServiceClass); }
        }

        public bool IsProxy
        {
            get { return this.HasService(Jacdac.ProxyConstants.ServiceClass); }
        }

        public bool IsDashboard
        {
            get { return this.HasService(Jacdac.DashboardConstants.ServiceClass); }
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