using Jacdac.Servers;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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
        public bool IsPassive = false;
        public bool IsProxy = false;
        public bool IsInfrastructure = false;
        public bool DisableRoleManager = false;
        public bool DisableLogger = false;
        public bool DisableUniqueBrain = false;
        public ISettingsStorage SettingsStorage;
        public ISettingsStorage RoleStorage;
        public JDServiceServer[] Services;
        public ControlAnnounceFlags StatusLight = Platform.StatusLight;
        public SetStatusLightHandler SetStatusLight = Platform.SetStatusLight;
        public ServiceSpecificationCatalog SpecificationCatalog;
        public LoggerPriority DefaultMinLoggerPriority = LoggerPriority.Silent;
    }

    public sealed partial class JDBus : JDNode, IDisposable
    {
        // updated concurrently, locked by this
        private JDDevice[] devices;
        private Transport[] transports;

        public readonly JDDeviceServer SelfDeviceServer;
        private readonly Clock clock;

        public bool IsPassive;
        public bool IsStreaming;
        public LoggerPriority DefaultMinLoggerPriority;

        public ServiceSpecificationCatalog SpecificationCatalog { get; set; }

        public TimeSpan LastResetInTime;
        private Timer announceTimer;

        /// <summary>
        /// Creates a new Jacdac bus with the given transport
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="options"></param>
        public JDBus(Transport transport, JDBusOptions options = null)
        {
            if (options == null)
                options = new JDBusOptions();

            this.clock = Platform.CreateClock();
            this.IsPassive = options.IsPassive;
            this.SpecificationCatalog = options.SpecificationCatalog;
            this.SelfDeviceServer = new JDDeviceServer(this, HexEncoding.ToString(options.DeviceId), options);
            this.DefaultMinLoggerPriority = options.DefaultMinLoggerPriority;

            this.devices = new JDDevice[] { new JDDevice(this, this.SelfDeviceServer.DeviceId) };
            this.transports = new Transport[0];
            if (transport != null)
                this.AddTransport(transport);
            this.Start();
        }

        public override JDBus Bus => this;

        /// <summary>
        /// Gets the logger server hosted on the self device if any
        /// </summary>
        public override LoggerServer Logger => this.SelfDeviceServer?.Logger;

        /// <summary>
        /// Gets the role manager server hosted on the self device, if any.
        /// </summary>
        public RoleManagerServer RoleManager { get { return this.SelfDeviceServer?.RoleManager; } }

        public override string ToString()
        {
            return $"bus ({this.devices.Length} devices)";
        }

        /// <summary>
        /// Generates a text description of the state of the bus
        /// </summary>
        /// <returns></returns>
        public string Describe()
        {
            var build = new StringBuilder();
            var transports = this.transports;
            var devices = this.devices;

            build.AppendLine($"bus {this.Timestamp}");
            build.AppendLine("transports:");
            foreach (var transport in transports)
            {
                build.AppendLine($"  {transport}");
            }
            build.AppendLine("devices:");
            foreach (var device in devices)
            {
                build.AppendLine($"  {device.ShortId} ({device.DeviceId})");
                var services = device.GetServices();
                foreach (var service in services)
                {
                    if (service.ServiceClass == 0)
                    {
                        service.GetRegister((ushort)ControlReg.Uptime);
                        service.GetRegister((ushort)ControlReg.ProductIdentifier);
                        service.GetRegister((ushort)ControlReg.FirmwareVersion);
                        service.GetRegister((ushort)ControlReg.DeviceDescription);
                    }
                    build.AppendLine($"      {service.ServiceIndex}: {service.Name} (0x{service.ServiceClass.ToString("x8")})");
                    var registers = service.GetRegisters();
                    foreach (var register in registers)
                        build.AppendLine($"        {register.Name}: {register.GetHumanValue()}");
                }
            }
            if (this.RoleManager != null)
            {
                build.AppendLine("clients:");
                var clients = this.RoleManager.Roles;
                foreach (var client in clients)
                    build.AppendLine($"  {client}");
            }
            return build.ToString();
        }

        /// <summary>
        /// Adds a transport to the bus
        /// </summary>
        /// <param name="transport"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddTransport(Transport transport)
        {
            if (transport == null)
                throw new ArgumentNullException("transport");

            var transports = this.transports;
            var newTransports = new Transport[transports.Length + 1];
            transports.CopyTo(newTransports, 0);
            newTransports[newTransports.Length - 1] = transport;

            this.transports = newTransports;

            transport.FrameReceived += Transport_FrameReceived;
            transport.ErrorReceived += Transport_ErrorReceived;

            if (this.Running)
                transport.Connect();
        }

        /// <summary>
        /// Sends a frame to the transports in the bus
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="sender"></param>
        internal void SendFrame(byte[] frame, Transport sender = null)
        {
            if (!Packet.CheckFrame(frame))
                return;
            var transports = this.transports;
            foreach (var transport in transports)
                if (transport != sender && transport.ConnectionState == ConnectionState.Connected)
                    transport.SendFrame(frame);
            this.FrameSent?.Invoke(sender, frame);
        }

        public void Start()
        {
            if (this.announceTimer == null)
            {
                this.LogDebug($"bus: start ({this.SelfDeviceServer.ShortId})");
                this.announceTimer = new System.Threading.Timer(this.handleSelfAnnounce, null, 100, 499);
            }
            this.Connect();
        }

        public bool Running
        {
            get { return this.announceTimer != null; }
        }

        public void Stop()
        {
            var at = this.announceTimer;
            if (at != null)
            {
                this.announceTimer = null;
                at.Dispose();
            }
            this.Disconnect();
        }

        public void Connect()
        {
            var transports = this.transports;
            foreach (var transport in transports)
                transport.Connect();
        }

        public void Disconnect()
        {
            var transports = this.transports;
            foreach (var transport in transports)
                transport.Disconnect();
        }

        public Transport[] Transports
        {
            get { return this.transports; }
        }

        private void Transport_FrameReceived(Transport sender, byte[] frame)
        {
            this.ProcessFrame(sender, frame);
        }

        public void ProcessFrame(Transport sender, byte[] frame)
        {
            // process packet
            var packets = Packet.FromFrame(frame);
            var timestamp = this.Timestamp;
            foreach (var packet in packets)
            {
                packet.Timestamp = timestamp;
                this.ProcessPacket(packet);
            }

            // broadcast to other transports
            if (packets.Length > 0)
                this.SendFrame(frame, sender);
        }

        private void Transport_ErrorReceived(Transport sender, TransportErrorReceivedEventArgs args)
        {
            var e = args.Error;
            string name = "?";
            switch (e)
            {
                case TransportError.Overrun: name = "overrun"; TransportStats.Overrun++; break;
                case TransportError.BufferFull: name = "overrun"; TransportStats.BufferFull++; break;
                case TransportError.Frame: name = "frame"; TransportStats.Frame++; break;
                case TransportError.Frame_MaxData: name = "frame max data"; TransportStats.FrameMaxData++; break;
                case TransportError.Frame_Busy: name = "frame busy"; TransportStats.FrameBusy++; break;
                case TransportError.Frame_A: name = "frame A"; TransportStats.FrameA++; break;
                case TransportError.Frame_B: name = "frame B"; TransportStats.FrameB++; break;
                case TransportError.Frame_C: name = "frame C"; TransportStats.FrameC++; break;
                case TransportError.Frame_D: name = "frame D"; TransportStats.FrameD++; break;
                case TransportError.Frame_E: name = "frame E"; TransportStats.FrameE++; break;
                case TransportError.Frame_F: name = "frame F"; TransportStats.FrameF++; break;
            }

            this.LogDebug($"transport error {name}");
        }
        void ProcessPacket(Packet pkt)
        {
            var deviceId = pkt.DeviceId;
            JDDevice device = null;
            if (!pkt.IsMultiCommand && !this.TryGetDevice(deviceId, out device))
                device = this.GetDevice(deviceId);

            if (device == null)
            {
                // skip
            }
            else if (pkt.IsCommand)
            {
                if (deviceId == this.SelfDeviceServer.DeviceId)
                {
                    if (pkt.RequiresAck)
                    {
                        var ack = Packet.From(pkt.Crc);
                        ack.ServiceIndex = Jacdac.Constants.JD_SERVICE_INDEX_CRC_ACK;
                        ack.DeviceId = this.SelfDeviceServer.DeviceId;
                        this.SelfDeviceServer.SendPacket(ack);
                    }
                    this.SelfDeviceServer.ProcessPacket(pkt);
                }
                device.ProcessPacket(pkt);
            }
            else
            {
                device.LastSeen = pkt.Timestamp;
                if (pkt.ServiceIndex == Jacdac.Constants.JD_SERVICE_INDEX_CTRL
                    && pkt.ServiceCommand == Jacdac.Constants.CMD_ADVERTISEMENT_DATA)
                {
                    device.ProcessAnnouncement(pkt);
                }
                else if (pkt.ServiceIndex == Jacdac.Constants.JD_SERVICE_INDEX_CTRL &&
                  pkt.IsMultiCommand &&
                  pkt.ServiceCommand == (Jacdac.Constants.CMD_SET_REG | Jacdac.Constants.CONTROL_REG_RESET_IN)
                  )
                {
                    // someone else is doing reset in
                    this.LastResetInTime = pkt.Timestamp;
                }
                else
                {
                    if (deviceId == this.SelfDeviceServer.DeviceId)
                        this.SelfDeviceServer.ProcessPacket(pkt);
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

        public JDDevice[] GetDevices()
        {
            return this.devices;
        }

        public TimeSpan Timestamp
        {
            get { return this.clock(); }
        }

        //public JDDevice SelfDevice
        // {
        //     get { return this.GetDevice(this.selfDeviceId); }
        // }

        private void handleSelfAnnounce(Object stateInfo)
        {
            this.SelfDeviceServer.SendAnnounce();
            this.SendResetIn();
            this.CleanDevices();
            this.RefreshRegisters();
            this.SelfAnnounced?.Invoke(this, EventArgs.Empty);
        }

        private void SendResetIn()
        {
            if ((this.Timestamp - this.LastResetInTime).TotalMilliseconds < Jacdac.Constants.RESET_IN_TIME_US / 2)
                return;

            // don't send reset if already received
            // or no devices
            this.LastResetInTime = this.Timestamp;
            var rst = Packet.From(
                (ushort)((ushort)Jacdac.Constants.CMD_SET_REG | (ushort)Jacdac.ControlReg.ResetIn),
                PacketEncoding.Pack((uint)Jacdac.Constants.RESET_IN_TIME_US)
                );
            rst.SetMultiCommand(ServiceClasses.Control);
            this.SelfDeviceServer.SendPacket(rst);
        }

        private void CleanDevices()
        {
            var now = this.Timestamp;
            var devices = this.devices;
            var disconnected = 0;
            foreach (var device in devices)
            {
                if (device.DeviceId != this.SelfDeviceServer.DeviceId &&
                    (now - device.LastSeen).TotalMilliseconds > Jacdac.Constants.JD_DEVICE_DISCONNECTED_DELAY)
                {
                    device.Disconnect();
                    disconnected++;
                }
            }
            if (disconnected > 0)
            {
                this.LogDebug($"cleaning out {disconnected} devices");
                var disco = new JDDevice[disconnected];
                var newDevices = new JDDevice[devices.Length - disconnected];
                var k = 0;
                var d = 0;
                for (var i = 0; i < devices.Length; i++)
                {
                    var device = devices[i];
                    if (device.Bus != null)
                        newDevices[k++] = device;
                    else
                        disco[d++] = device;
                }
                System.Diagnostics.Debug.Assert(d == disconnected);
                System.Diagnostics.Debug.Assert(k == newDevices.Length);
                this.devices = newDevices;

                var roleMgr = this.SelfDeviceServer.RoleManager;
                if (roleMgr != null)
                    roleMgr.BindRoles();
                var ev = this.DeviceDisconnected;
                if (ev != null)
                    for (var i = 0; i < disco.Length; i++)
                        ev.Invoke(this, new DeviceEventArgs(disco[i]));
            }
        }

        private void RefreshRegisters()
        {
            var devices = this.GetDevices();
            foreach (var device in devices)
            {
                if (device.HasService(ServiceClasses.Proxy)) continue;

                var services = device.GetServices();
                foreach (var service in services)
                {
                    var registers = service.GetRegisters();
                    foreach (var register in registers)
                    {
                        if (!register.NotImplemented
                            || register.IsStreaming
                            || register.NeedsRefresh
                            || register.HasChangedListeners()
                            || register.Data == null
                            || this.IsStreaming)
                            register.RefreshMaybe();
                    }
                }
            }
        }

        public event DeviceEventHandler DeviceConnected;

        public event DeviceEventHandler DeviceDisconnected;

        public event NodeEventHandler SelfAnnounced;

        public event FrameEventHandler FrameSent;

        public void Dispose()
        {
            this.Stop();
            var transports = this.transports;
            this.transports = new Transport[0];
            foreach (var transport in transports)
                transport.Dispose();
        }
    }
}
