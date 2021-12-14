namespace Jacdac
{
    public sealed class JDServerServiceProvider
    {
        public readonly JDBus Bus;
        private byte restartCounter = 0;
        private byte packetCount = 0;
        private JDServiceServer[] servers;

        public JDServerServiceProvider(JDBus bus, JDBusOptions options)
        {
            this.Bus = bus;
            this.servers = new JDServiceServer[1 + (options.Services != null ? options.Services.Length : 0)];
            this.servers[0] = new ControlServer(options);
            if (options.Services != null)
                options.Services.CopyTo(this.servers, 1);
            for (byte i = 0; i < servers.Length; i++)
            {
                var server = this.servers[i];
                server.Device = this;
                server.ServiceIndex = i;
            }
        }

        public void SendPacket(Packet pkt)
        {
            if (!pkt.IsMultiCommand)
                pkt.DeviceId = this.Bus.SelfDevice.DeviceId;
            this.Bus.SendPacket(pkt);
        }
    }
}
