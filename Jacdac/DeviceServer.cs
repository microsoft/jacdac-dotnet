using Jacdac.Servers;
using System;

namespace Jacdac
{
    public sealed partial class JDDeviceServer
    {
        public readonly JDBus Bus;
        public readonly string DeviceId;
        public readonly ControlServer Control;
        public readonly RoleManagerServer RoleManager;
        private readonly ControlAnnounceFlags statusLight;
        private byte restartCounter = 0;
        private byte packetCount = 0;
        private JDServiceServer[] services;
        public bool IsClient;
        private ushort eventCounter = 0;

        public JDDeviceServer(JDBus bus, string deviceId, JDBusOptions options)
        {
            this.Bus = bus;
            this.statusLight = options != null ? options.StatusLight : ControlAnnounceFlags.StatusLightNone;
            this.DeviceId = deviceId;
            this.IsClient = options.IsClient;
            this.services = new JDServiceServer[1 + (options.RoleManager ? 1 : 0) + (options.Services != null ? options.Services.Length : 0)];
            var k = 0;
            this.services[k++] = this.Control = new ControlServer(options);
            if (options.RoleManager)
                this.services[k++] = this.RoleManager = new RoleManagerServer();
            if (options.Services != null)
                options.Services.CopyTo(this.services, k);
            for (byte i = 0; i < services.Length; i++)
            {
                var server = this.services[i];
                server.Device = this;
                server.ServiceIndex = i;
            }
        }

        public string ShortId
        {
            get { return Util.ShortDeviceId(this.DeviceId); }
        }

        public override string ToString()
        {
            return this.ShortId;
        }

        public void ProcessPacket(Packet pkt)
        {
            var services = this.services;
            if (pkt.IsMultiCommand)
            {
                var serviceClass = pkt.MulticommandClass;
                for (var i = 0; i < services.Length; ++i)
                {
                    var service = services[i];
                    if (service.ServiceClass == serviceClass)
                        service.ProcessPacket(pkt);
                }
            }
            else
            {
                if (pkt.ServiceIndex < services.Length)
                {
                    var service = services[pkt.ServiceIndex];
                    if (!service.ProcessPacket(pkt))
                    {

                    }
                }
            }

        }

        public void SendPacket(Packet pkt)
        {
            this.packetCount++;
            var frame = Packet.ToFrame(new Packet[] { pkt });
            this.Bus.SendFrame(frame);
        }

        public void SendAnnounce()
        {
            // we do not support any services (at least yet)
            if (this.restartCounter < 0xf) this.restartCounter++;

            var services = this.services;
            var data = new byte[services.Length * 4 + 1];
            Util.Write16(data, 0, (ushort)((ushort)this.restartCounter |
                        (this.IsClient ? (ushort)ControlAnnounceFlags.IsClient : (ushort)0) |
                        (ushort)ControlAnnounceFlags.SupportsBroadcast |
                        (ushort)ControlAnnounceFlags.SupportsFrames |
                        (ushort)ControlAnnounceFlags.SupportsACK |
                        (ushort)this.statusLight
                    ));
            data[2] = this.packetCount;
            // 3 reserved
            for (uint i = 1; i < services.Length; ++i)
                Util.Write32(data, i * 4, services[i].ServiceClass);
            this.packetCount = 0;
            var pkt = Packet.From(Jacdac.Constants.CMD_ADVERTISEMENT_DATA, data);
            pkt.ServiceIndex = Jacdac.Constants.JD_SERVICE_INDEX_CTRL;
            pkt.DeviceId = this.DeviceId;
            this.SendPacket(pkt);
        }

        public ushort CreateEventCmd(ushort evCode)
        {
            if ((evCode >> 8) != 0)
                throw new ArgumentException("invalid event code");

            this.eventCounter = (ushort)((this.eventCounter + 1) & Jacdac.Constants.CMD_EVENT_COUNTER_MASK);
            return (ushort)(
                Jacdac.Constants.CMD_EVENT_MASK |
                (this.eventCounter << Jacdac.Constants.CMD_EVENT_COUNTER_POS) |
                evCode
            );
        }

        public void DelayedSendPacket(Packet pkt, int delay)
        {
            var timer = new System.Threading.Timer((state) =>
            {
                this.SendPacket(pkt);
            }, null, delay, System.Threading.Timeout.Infinite);
        }
    }
}
