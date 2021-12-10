using System;
using System.Collections;

namespace Jacdac
{
    public sealed class JDBus : JDNode
    {
        // updated concurrently, locked by this
        private readonly ArrayList devices;
        private readonly Transport transport;
        private readonly string selfDeviceId;

        private TimeSpan _lastResetInTime;

        public JDBus(Transport transport)
        {
            this.devices = new ArrayList();
            this.selfDeviceId = Platform.DeviceId();

            this.transport = transport;
            this.transport.PacketReceived += Transport_PacketReceived;
            this.transport.ErrorReceived += Transport_ErrorReceived;

            this.transport.Connect();
        }

        private void Transport_PacketReceived(Transport sender, Packet packet)
        {
            this.ProcessPacket(packet);
        }

        private void Transport_ErrorReceived(Transport sender, TransportErrorReceivedEventArgs args)
        {
            // TODO
        }

        void ProcessPacket(Packet pkt)
        {
            JDDevice device = null;
            if (!pkt.IsMultiCommand && !this.TryGetDevice(pkt.DeviceId, out device)) return; // ignore unknown device or invalid multicommand packet

            var isAnnounce = false;
            if (device == null)
            {
                // skip
            }
            else if (pkt.IsCommand)
            {
                if (pkt.DeviceId == this.selfDeviceId)
                {
                    if (pkt.RequiresAck)
                    {
                        var ack = Packet.OnlyHeader(pkt.Crc);
                        ack.ServiceIndex = Jacdac.Constants.JD_SERVICE_INDEX_CRC_ACK;
                        ack.DeviceId = this.selfDeviceId;
                        this.SendPacket(pkt);
                    }
                }
                device.ProcessPacket(pkt);
            }
            else
            {
                device.LastSeen = pkt.Timestamp;
                if (pkt.ServiceIndex == Jacdac.Constants.JD_SERVICE_INDEX_CTRL)
                {
                    if (pkt.ServiceCommand == Jacdac.Constants.CMD_ADVERTISEMENT_DATA)
                    {
                        isAnnounce = true;
                        device.ProcessAnnouncement(pkt);
                    }
                    else if (
                      pkt.IsMultiCommand &&
                      pkt.ServiceCommand == (Jacdac.Constants.CMD_SET_REG | Jacdac.Constants.CONTROL_REG_RESET_IN)
                  )
                    {
                        // someone else is doing reset in
                        this._lastResetInTime = pkt.Timestamp;
                    }
                }
                device.ProcessPacket(pkt);
            }
            // this.emit(PACKET_PROCESS, pkt)
            // don't spam with duplicate advertisement events
            if (isAnnounce)
            {
                // this.emit(PACKET_RECEIVE_ANNOUNCE, pkt)
            }
            else
            {
                //   this.emit(PACKET_RECEIVE, pkt)
                //if (pkt.isEvent) this.emit(PACKET_EVENT, pkt)
                //else if (pkt.isReport) this.emit(PACKET_REPORT, pkt)
            }
        }

        public bool TryGetDevice(string deviceId, out JDDevice device)
        {
            lock (this)
            {
                for (var i = 0; i < this.devices.Count; i++)
                {
                    var d = (JDDevice)this.devices[i];
                    if (d.DeviceId == deviceId)
                    {
                        device = d;
                        return true;
                    }
                }
                device = null;
                return false;
            }
        }

        public JDDevice Device(string deviceId)
        {
            lock (this)
            {
                JDDevice device;
                if (!this.TryGetDevice(deviceId, out device))
                {
                    device = new JDDevice(this, deviceId);
                    this.devices.Add(device);
                }
                // sorting?
                //this.devices.Sort(new JDDevice.Comparer());
                this.RaiseChanged();
                return device;
            }
        }

        public JDDevice[] Devices()
        {
            lock (this)
            {
                var res = (JDDevice[])this.devices.ToArray();
                return res;
            }
        }

        public TimeSpan Timestamp
        {
            get { return Platform.Now(); }
        }

        public JDDevice SelfDevice
        {
            get { return this.Device(this.selfDeviceId); }
        }

        internal void SendPacket(Packet pkt)
        {
            if (this.transport.ConnectionState != ConnectionState.Connected)
                return;

            this.transport.SendPacket(pkt);
        }
    }
}
