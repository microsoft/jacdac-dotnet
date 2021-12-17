using System;
using System.Collections;

namespace Jacdac
{
    public sealed class JDDeviceServer
    {
        public readonly JDBus Bus;
        public readonly string DeviceId;
        private byte restartCounter = 0;
        private byte packetCount = 0;
        private JDServiceServer[] services;
        public bool IsClient;
        private ushort eventCounter = 0;

        public JDDeviceServer(JDBus bus, string deviceId, JDBusOptions options)
        {
            this.Bus = bus;
            this.DeviceId = deviceId;
            this.IsClient = options.IsClient;
            this.services = new JDServiceServer[1 + (options.Services != null ? options.Services.Length : 0)];
            this.services[0] = new ControlServer(options);
            if (options.Services != null)
                options.Services.CopyTo(this.services, 1);
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
            this.Bus.Transport.SendFrame(frame);
        }

        public void SendAnnounce()
        {
            // we do not support any services (at least yet)
            if (this.restartCounter < 0xf) this.restartCounter++;

            var servers = this.services;
            var serviceClasses = new object[servers.Length - 1];
            for (var i = 1; i < servers.Length; ++i)
                serviceClasses[i - 1] = new object[] { servers[i].ServiceClass };
            var reserved = 0u;
            var data = PacketEncoding.Pack("u16 u8 u8 r: u32",
                new object[] {
                    (uint)((ushort)this.restartCounter |
                        (this.IsClient ? (ushort)ControlAnnounceFlags.IsClient : (ushort)0) |
                        (ushort)ControlAnnounceFlags.SupportsBroadcast |
                        (ushort)ControlAnnounceFlags.SupportsFrames |
                        (ushort)ControlAnnounceFlags.SupportsACK
                    ),
                    (uint)this.packetCount,
                    reserved,
                    serviceClasses
                });
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
