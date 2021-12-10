using System;
using System.Collections;

namespace Jacdac
{
    public sealed class JDDevice : JDNode
    {
        public readonly JDBus Bus;
        public readonly string DeviceId;
        public readonly string ShortId;
        public TimeSpan LastSeen;

        byte[] _servicesData;
        JDService[] _services = null;

        public JDDevice(JDBus bus, string deviceId)
        {
            this.Bus = bus;
            this.DeviceId = deviceId;
            this.ShortId = Util.ShortDeviceId(this.DeviceId);
            this.LastSeen = bus.Timestamp;
            this._servicesData = new byte[0];
        }

        public override string ToString()
        {
            return this.ShortId;
        }

        public void ProcessPacket(Packet pkt)
        {
            //this.lost = false
            //this.emit(PACKET_RECEIVE, pkt)
            //if (pkt.IsReport) this.emit(PACKET_REPORT, pkt)
            //else if (pkt.IsEvent) this.emit(PACKET_EVENT, pkt)

            var srvs = this._services;
            var i = pkt.ServiceIndex;
            if (srvs != null && i < srvs.Length)
                srvs[i].ProcessPacket(pkt);
        }

        public void ProcessAnnouncement(Packet pkt)
        {
            var data = pkt.Data;
            var changed = false;
            uint w0 = this._servicesData.Length == 0
                ? 0
                : Util.GetNumber(this._servicesData, NumberFormat.UInt32LE, 0);
            uint w1 = data.Length == 0 ? 0 : Util.GetNumber(data, NumberFormat.UInt32LE, 0);

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
                for(var i = 1; i < res.Length;++i)
                {
                    res[i] = Util.Read32(data, i * 4);
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
                var sc = Util.Read32(data, i * 4);
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

        public JDService[] Services()
        {
            var srvs = this._services.Clone() as JDService[];
            return srvs;
        }

        public void SendPacket(Packet pkt)
        {
            pkt.DeviceId = this.DeviceId;
            this.Bus.SendPacket(pkt);
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