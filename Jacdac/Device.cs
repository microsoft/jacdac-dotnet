using System;
using System.Collections;

namespace Jacdac {
    public sealed class JDDevice {
        public readonly JDBus Bus;
        public readonly string DeviceId;
        public TimeSpan LastSeen;
        byte[] serviceData;

        public JDDevice(JDBus bus, string deviceId) {
            this.Bus = bus;
            this.DeviceId = deviceId;
            this.LastSeen = bus.Timestamp;
            this.serviceData = new byte[0];
        }

        public void ProcessPacket(Packet pkt)
        {

        }

        public void ProcessAnnouncement(Packet pkt)
        {

        }

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
}