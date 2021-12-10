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

        byte[] _servicesData = null;
        JDService[] _services = null;

        public JDDevice(JDBus bus, string deviceId)
        {
            this.Bus = bus;
            this.DeviceId = deviceId;
            this.ShortId = Util.ShortDeviceId(this.DeviceId);
            this.LastSeen = bus.Timestamp;
            this._servicesData = null;
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
            uint w0 = this._servicesData != null
                ? Util.GetNumber(this._servicesData, NumberFormat.UInt32LE, 0)
                : 0;
            uint w1 = Util.GetNumber(pkt.Data, NumberFormat.UInt32LE, 0);

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
                byte n = (byte)(this._servicesData.Length >> 2);
                var s = new JDService[n];
                for (byte i = 0; i < n; ++i)
                {
                    var sc = i == 0 ? 0 : Util.Read32(this._servicesData, i * 4);
                    s[i] = new JDService(this, i, sc);
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
    public delegate void DeviceEventHandler(JDNode sensor, DeviceEventArgs e);
}