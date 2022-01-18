using System;

namespace Jacdac.Servers
{
    public interface IAzureIoTHubHealth
    {
        string HubName { get; }
        string HubDeviceId { get; }
        AzureIotHubHealthConnectionStatus ConnectionStatus { get; }

        void Connect();
        void Disconnect();

        void SetConnectionString(string connectionString);

        event EventHandler ConnectionStatusChanged;

        event EventHandler MessageSent;
    }

    public sealed class AzureIotHubHealthServer : JDServiceServer
    {
        private IAzureIoTHubHealth hub;
        private readonly JDDynamicRegisterServer hubNameRegister;
        private readonly JDDynamicRegisterServer hubDeviceIdRegister;
        private readonly JDDynamicRegisterServer connectionStatusRegister;

        public AzureIotHubHealthServer(IAzureIoTHubHealth hub, JDServiceServerOptions options = null)
            : base(ServiceClasses.AzureIotHubHealth, options)
        {
            this.hub = hub;

            this.AddRegister(this.hubNameRegister = new JDDynamicRegisterServer((ushort)AzureIotHubHealthReg.HubName, AzureIotHubHealthRegPack.HubName, (server) => new object[] { this.hub.HubName }));
            this.AddRegister(this.hubDeviceIdRegister = new JDDynamicRegisterServer((ushort)AzureIotHubHealthReg.HubDeviceId, AzureIotHubHealthRegPack.HubDeviceId, (server) => new object[] { this.hub.HubDeviceId }));
            this.AddRegister(this.connectionStatusRegister = new JDDynamicRegisterServer((ushort)AzureIotHubHealthReg.ConnectionStatus, AzureIotHubHealthRegPack.ConnectionStatus, (server) => new object[] { this.hub.ConnectionStatus }));

            // restricted command
            this.AddCommand((ushort)AzureIotHubHealthCmd.SetConnectionString, this.handleSetConnectionString);

            this.hub.MessageSent += handleMessageSent;
            this.hub.ConnectionStatusChanged += handleConnectionStatusChanged;
        }

        public override string ToString()
        {
            return this.hub == null ? "disposed" : $"{this.hub.HubName} in {this.hub.HubDeviceId}: {this.hub.ConnectionStatus}";
        }

        private void handleConnectionStatusChanged(object sender, EventArgs e)
        {
            if (this.hub == null)
                throw new ObjectDisposedException("server");

            var payload = PacketEncoding.Pack(AzureIotHubHealthEventPack.ConnectionStatusChange, new object[] { this.hub.ConnectionStatus });
            this.SendEvent((ushort)AzureIotHubHealthEvent.ConnectionStatusChange, payload);
        }

        private void handleMessageSent(object sender, EventArgs e)
        {
            if (this.hub == null)
                throw new ObjectDisposedException("server");

            this.SendEvent((ushort)AzureIotHubHealthEvent.MessageSent);
        }

        private void handleSetConnectionString(JDNode sender, PacketEventArgs args)
        {
            if (this.hub == null)
                throw new ObjectDisposedException("server");

            var value = PacketEncoding.UnPack(AzureIotHubHealthCmdPack.SetConnectionString, args.Packet.Data);
            var connectionString = (string)value[0];

            this.hub.SetConnectionString(connectionString);
        }

        public override void Dispose()
        {
            var hub = this.hub;
            if (hub != null)
            {
                this.hub = null;
                hub.MessageSent -= handleMessageSent;
                hub.ConnectionStatusChanged -= handleConnectionStatusChanged;
            }
        }
    }
}
