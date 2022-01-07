using System;

namespace Jacdac.Servers
{
    public sealed partial class LoggerServer : JDServiceServer
    {
        public readonly JDStaticRegisterServer MinPriority;
        private TimeSpan lastListenerTime = TimeSpan.Zero;

        internal LoggerServer(LoggerPriority minPriority)
            : base(ServiceClasses.Logger, null)
        {
            this.MinPriority = this.AddRegister((ushort)LoggerReg.MinPriority, LoggerRegPack.MinPriority, new object[] { minPriority });
        }

        public void SendReport(LoggerPriority priority, string message)
        {
            LoggerCmd cmd;
            switch (priority)
            {
                case LoggerPriority.Debug: cmd = LoggerCmd.Debug; break;
                case LoggerPriority.Log: cmd = LoggerCmd.Log; break;
                case LoggerPriority.Error: cmd = LoggerCmd.Error; break;
                case LoggerPriority.Warning: cmd = LoggerCmd.Warn; break;
                default: return;
            }

            var bus = this.Device.Bus;
            var now = bus.Timestamp;
            if ((now - this.lastListenerTime).TotalMilliseconds > Constants.LOGGER_LISTENER_TIMEOUT)
            {
                this.MinPriority.SetValues(new object[] { bus.DefaultMinLoggerPriority });
                this.lastListenerTime = TimeSpan.Zero;
            }

            var minPriority = (LoggerPriority)this.MinPriority.GetValues()[0];
            if (message == null || message.Length == 0 || this.lastListenerTime == TimeSpan.Zero || priority < minPriority)
                return;

            var i = 0;
            while (i < message.Length)
            {
                var chunk = message.Substring(i, Math.Min((int)Constants.JD_SERIAL_MAX_PAYLOAD_SIZE, (int)(message.Length - i - Constants.JD_SERIAL_MAX_PAYLOAD_SIZE)));
                var payload = PacketEncoding.Pack(LoggerCmdPack.Log, new object[] { chunk });
                this.SendPacket(Packet.From((ushort)cmd, payload));
                i += chunk.Length;
            }
        }
    }
}
