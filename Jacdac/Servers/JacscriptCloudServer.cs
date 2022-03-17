using System;

namespace Jacdac.Servers
{
    [Serializable]
    public sealed class TwinChangedEventsArgs
    {
        public string Path { get; private set; }
        public double Value { get; private set; }
        public TwinChangedEventsArgs(string path, double value)
        {
            this.Path = path;
            this.Value = value;
        }
    }
    public delegate void TwinChangedEventHandler(IJacscriptCloud sender, TwinChangedEventsArgs args);

    [Serializable]
    public sealed class CloudCommandEventArgs
    {
        public uint SequenceNumber { get; private set; }
        public string Command { get; private set; }
        public double[] Arguments { get; private set; }
        public CloudCommandEventArgs(uint sequenceNumber, string command, double[] arguments)
        {
            this.SequenceNumber = sequenceNumber;
            this.Command = command;
            this.Arguments = arguments;
        }
    }
    public delegate void CloudCommandEventHandler(IJacscriptCloud cloud, CloudCommandEventArgs args);

    public interface IJacscriptCloud
    {
        bool IsConnected { get; }
        void Upload(string label, double[] value);
        bool TryGetTwin(string path, out double current);
        void SubscribeTwin(string path);
        void AckCloudCommand(uint sequenceNumber, double[] result);

        event TwinChangedEventHandler TwinChanged;
        event CloudCommandEventHandler CloudCommand;
    }

    /// <summary>
    /// A server implementation of the Jascript cloud service
    /// </summary>
    public sealed class JacscriptCloudServer : JDServiceServer
    {
        readonly IJacscriptCloud Cloud;
        readonly JDDynamicRegisterServer connectedRegister;

        public JacscriptCloudServer(IJacscriptCloud cloud, JDServiceServerOptions options = null) : base(ServiceClasses.JacscriptCloud, options)
        {
            this.Cloud = cloud;
            this.AddRegister(this.connectedRegister = new JDDynamicRegisterServer((ushort)JacscriptCloudReg.Connected, JacscriptCloudRegPack.Connected, (args) => new object[] { this.Cloud.IsConnected }));
            this.AddCommand((ushort)JacscriptCloudCmd.Upload, this.handleUpload);
            this.AddCommand((ushort)JacscriptCloudCmd.GetTwin, this.handleGetTwin);
            this.AddCommand((ushort)JacscriptCloudCmd.AckCloudCommand, this.handleAckCloudCommand);

            this.Cloud.TwinChanged += handleTwinChange;
            this.Cloud.CloudCommand += handleCloudCommand;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.Cloud.TwinChanged -= handleTwinChange;
            this.Cloud.CloudCommand -= handleCloudCommand;
        }

        private void handleCloudCommand(IJacscriptCloud cloud, CloudCommandEventArgs args)
        {
            var seqno = args.SequenceNumber;
            var command = args.Command;
            var arguments = new object[args.Arguments.Length];
            for (var i = 0; i < args.Arguments.Length; ++i)
                arguments[i] = new object[] { args.Arguments[i] };
            var data = PacketEncoding.Pack(JacscriptCloudEventPack.CloudCommand, new object[] { seqno, command, arguments });
            this.SendEvent((ushort)JacscriptCloudEvent.TwinChange, data);
        }

        private void handleTwinChange(IJacscriptCloud sender, TwinChangedEventsArgs args)
        {
            this.SendEvent((ushort)JacscriptCloudEvent.TwinChange);
        }

        private void handleUpload(JDNode sender, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var data = pkt.Data;
            var values = PacketEncoding.UnPack(JacscriptCloudCmdPack.Upload, data);
            var label = (string)values[0];
            var value = (double[])values[1];

            this.Cloud.Upload(label, value);
        }

        private void handleGetTwin(JDNode sender, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var data = pkt.Data;
            var values = PacketEncoding.UnPack(JacscriptCloudCmdPack.GetTwin, data);
            var path = (string)values[0];

            double current;
            if (this.Cloud.TryGetTwin(path, out current))
            {
                var reportPayload = PacketEncoding.Pack(JacscriptCloudCmdPack.GetTwinReport, new object[] { path, current });
                var reportPkt = Packet.From((ushort)JacscriptCloudCmd.GetTwin, reportPayload);
                this.Device.SendPacket(reportPkt);
            }
        }

        private void handleAckCloudCommand(JDNode sender, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var data = pkt.Data;
            var values = PacketEncoding.UnPack(JacscriptCloudCmdPack.AckCloudCommand, data);
            var seqNo = (uint)values[0];
            var res = (object[])values[1];
            var results = new double[res.Length];
            for (var i = 0; i < res.Length; i++)
                results[i] = (double)(((object[])res[i])[0]);
            this.Cloud.AckCloudCommand(seqNo, results);
        }
    }
}