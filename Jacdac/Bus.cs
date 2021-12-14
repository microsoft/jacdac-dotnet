using System;
using System.Diagnostics;
using System.Threading;

namespace Jacdac
{
    public sealed class JDBusOptions
    {
        public byte[] DeviceId = Platform.DeviceId;
        public string Description = Platform.DeviceDescription;
        public string FirmwareVersion = Platform.FirmwareVersion;
        public uint ProductIdentifier;
        public bool IsClient = true;
        public JDServiceServer[] Services;
    }

    public sealed class JDBus : JDNode
    {
        // updated concurrently, locked by this
        private JDDevice[] devices;

        private readonly Clock clock;
        private readonly string selfDeviceId;
        private byte restartCounter = 0;
        private byte packetCount = 0;
        private JDServiceServer[] servers;

        private readonly Transport transport;
        public bool IsClient;

        public TimeSpan LastResetInTime;
        private Timer announceTimer;

        public JDBus(Transport transport, JDBusOptions options = null)
        {
            if (options == null)
                options = new JDBusOptions();

            this.selfDeviceId = HexEncoding.ToString(options.DeviceId);
            this.clock = Platform.CreateClock();
            this.IsClient = options.IsClient;

            this.devices = new JDDevice[] { new JDDevice(this, this.selfDeviceId) };
            this.servers = new JDServiceServer[1 + (options.Services != null ? options.Services.Length : 0)];
            this.servers[0] = new ControlServer(options);
            if (options.Services != null)
                options.Services.CopyTo(this.servers, 1);
            for(byte i = 0; i < servers.Length; i++)
            {
                var server = this.servers[i];
                server.Bus = this;
                server.ServiceIndex = i;
            }

            this.transport = transport;
            this.transport.FrameReceived += Transport_FrameReceived;
            this.transport.ErrorReceived += Transport_ErrorReceived;

            this.transport.Connect();
        }

        private void Transport_FrameReceived(Transport sender, byte[] frame)
        {
            var packets = Packet.FromFrame(frame);
            var timestamp = this.Timestamp;
            foreach (var packet in packets)
            {
                packet.Timestamp = timestamp;
                this.ProcessPacket(packet);
            }
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

        private void Transport_ErrorReceived(Transport sender, TransportErrorReceivedEventArgs args)
        {
            Debug.WriteLine($"transport error {args.Error}");
            if (args.Data != null)
            {
                Debug.WriteLine(HexEncoding.ToString(args.Data));
            }
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
                {
                    if (pkt.DeviceId == this.selfDeviceId)
                    {
                        if (pkt.ServiceIndex < this.servers.Length)
                        {
                            var server = this.servers[pkt.ServiceIndex];
                            server.ProcessPacket(pkt);
                        }
                    }
                    device.ProcessPacket(pkt);
                }
            }
        }

        public bool TryGetDevice(string deviceId, out JDDevice device)
        {
            var devices = this.devices;
            for (var i = 0; i < devices.Length; i++)
            {
                var d = (JDDevice)devices[i];
                if (d.DeviceId == deviceId)
                {
                    device = d;
                    return true;
                }
            }
            device = null;
            return false;
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
                    var newDevices = new JDDevice[this.devices.Length + 1];
                    this.devices.CopyTo(newDevices, 0);
                    newDevices[newDevices.Length - 1] = device;
                    this.devices = newDevices;
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
            var devices = this.devices;
            var res = (JDDevice[])devices.Clone();
            return res;
        }

        public JDServiceServer[] Servers()
        {
            return (JDServiceServer[])this.servers.Clone();
        }

        public TimeSpan Timestamp
        {
            get { return this.clock(); }
        }

        public JDDevice SelfDevice
        {
            get { return this.GetDevice(this.selfDeviceId); }
        }

        internal void SendPacket(Packet pkt)
        {
            if (this.transport.ConnectionState != ConnectionState.Connected)
                return;
            this.packetCount++;
            var frame = Packet.ToFrame(new Packet[] { pkt });
            this.transport.SendFrame(frame);
        }

        private void SendAnnounce(Object stateInfo)
        {
            // we do not support any services (at least yet)
            if (this.restartCounter < 0xf) this.restartCounter++;

            var servers = this.servers;
            var serviceClasses = new object[servers.Length - 1];
            for (var i = 1; i < servers.Length; ++i)
                serviceClasses[i - 1] = new object[] { servers[i].ServiceClass };
            var data = PacketEncoding.Pack("u16 u8 x[8] r: u32",
                new object[] {
                    (ushort)((ushort)this.restartCounter |
                        (this.IsClient ? (ushort)ControlAnnounceFlags.IsClient : (ushort)0) |
                        (ushort)ControlAnnounceFlags.SupportsBroadcast |
                        (ushort)ControlAnnounceFlags.SupportsFrames |
                        (ushort)ControlAnnounceFlags.SupportsACK
                    ),
                    this.packetCount,
                    serviceClasses
                });
            this.packetCount = 0;
            var pkt = Packet.From(Jacdac.Constants.CMD_ADVERTISEMENT_DATA, data);
            pkt.ServiceIndex = Jacdac.Constants.JD_SERVICE_INDEX_CTRL;
            var selfDevice = this.SelfDevice;
            selfDevice.SendPacket(pkt);

            this.SelfAnnounced?.Invoke(this, EventArgs.Empty);
        }

        public event DeviceEventHandler DeviceConnected;

        public event DeviceEventHandler DeviceDisconnected;

        public event NodeEventHandler SelfAnnounced;
    }
}
