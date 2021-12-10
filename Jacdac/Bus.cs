using System;
using System.Collections;
using System.Threading;

namespace Jacdac
{
    public sealed class JDBus : JDNode
    {
        // updated concurrently, locked by this
        private readonly ArrayList devices;
        private readonly Transport transport;
        private readonly string selfDeviceId;
        public byte RestartCounter = 0;

        public TimeSpan LastResetInTime;
        private Timer announceTimer;

        public JDBus(Transport transport)
        {
            this.devices = new ArrayList();
            this.selfDeviceId = Platform.DeviceId();

            this.transport = transport;
            this.transport.PacketReceived += Transport_PacketReceived;
            this.transport.ErrorReceived += Transport_ErrorReceived;

            this.transport.Connect();
            this.Start();
        }

        public void Start()
        {
            if (this.announceTimer == null)
            {
                this.announceTimer = new System.Threading.Timer(this.SendAnnounce, null, 100, 499);
            }
        }

        public void Stop()
        {
            if (this.announceTimer != null)
            {
                this.announceTimer.Dispose();
                this.announceTimer = null;
            }
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
            if (!pkt.IsMultiCommand && !this.TryGetDevice(pkt.DeviceId, out device))
                device = this.GetDevice(pkt.DeviceId);

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
                if (pkt.ServiceIndex == Jacdac.Constants.JD_SERVICE_INDEX_CTRL && pkt.ServiceCommand == Jacdac.Constants.CMD_ADVERTISEMENT_DATA)
                {
                    device.ProcessAnnouncement(pkt);
                }
                else if (
                pkt.ServiceIndex == Jacdac.Constants.JD_SERVICE_INDEX_CTRL &&
                  pkt.IsMultiCommand &&
                  pkt.ServiceCommand == (Jacdac.Constants.CMD_SET_REG | Jacdac.Constants.CONTROL_REG_RESET_IN)
              )
                {
                    // someone else is doing reset in
                    this.LastResetInTime = pkt.Timestamp;
                }
                else
                    device.ProcessPacket(pkt);
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

        public JDDevice GetDevice(string deviceId)
        {
            var changed = false;
            JDDevice device;
            lock (this)
            {
                if (!this.TryGetDevice(deviceId, out device))
                {
                    device = new JDDevice(this, deviceId);
                    this.devices.Add(device);
                    changed = true;
                }
            }
            if (changed)
            {
                if (this.DeviceConnected != null)
                    this.DeviceConnected.Invoke(this, new DeviceEventArgs(device));
                this.RaiseChanged();
            }
            return device;
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
            get { return this.GetDevice(this.selfDeviceId); }
        }

        internal void SendPacket(Packet pkt)
        {
            if (this.transport.ConnectionState != ConnectionState.Connected)
                return;

            this.transport.SendPacket(pkt);
        }

        private void SendAnnounce(Object stateInfo)
        {
            // we do not support any services (at least yet)
            if (this.RestartCounter < 0xf) this.RestartCounter++;
            var data = new byte[2];
            Util.Write16(data, 0, (ushort)(this.RestartCounter |
                    (ushort)ControlAnnounceFlags.IsClient |
                    (ushort)ControlAnnounceFlags.SupportsBroadcast |
                    (ushort)ControlAnnounceFlags.SupportsFrames |
                    (ushort)ControlAnnounceFlags.SupportsACK));
            var pkt = Packet.From(Jacdac.Constants.CMD_ADVERTISEMENT_DATA, data);
            pkt.ServiceIndex = Jacdac.Constants.JD_SERVICE_INDEX_CTRL;
            this.SelfDevice.SendPacket(pkt);
            this.SelfAnnounce?.Invoke(this, EventArgs.Empty);
        }

        public event DeviceEventHandler DeviceConnected;

        public event DeviceEventHandler DeviceDisconnected;

        public event NodeEventHandler SelfAnnounce;
    }
}
