using System;

namespace Jacdac.Servers
{
    [Serializable]
    public sealed class LoggerEventArgs
    {
        public readonly LoggerPriority Priority;
        public readonly string Message;

        internal LoggerEventArgs(LoggerPriority priority, string message)
        {
            this.Priority = priority;
            this.Message = message;
        }

    }

    public delegate void LoggerEventHandler(JDNode sender, LoggerEventArgs e);

    /// <summary>
    /// A server implementation of the logger service.
    /// </summary>
    public sealed partial class LoggerServer : JDServiceServer
    {
        public readonly JDStaticRegisterServer MinPriorityRegister;
        private TimeSpan lastListenerTime = TimeSpan.Zero;

        internal LoggerServer(LoggerPriority minPriority)
            : base(ServiceClasses.Logger, null)
        {
            this.MinPriorityRegister = this.AddRegister((ushort)LoggerReg.MinPriority, LoggerRegPack.MinPriority, new object[] { (byte)minPriority });
        }

        public LoggerPriority MinPriority
        {
            get { return (LoggerPriority)(uint)this.MinPriorityRegister.GetValues()[0]; }
        }

        /// <summary>
        /// Raised when a new log event is added locally
        /// </summary>
        public event LoggerEventHandler LogEvent;

        public void SendReport(LoggerPriority priority, string message)
        {
            var bus = this.Device?.Bus;
            if (bus == null) return;

            LoggerCmd cmd;
            switch (priority)
            {
                case LoggerPriority.Debug: cmd = LoggerCmd.Debug; break;
                case LoggerPriority.Log: cmd = LoggerCmd.Log; break;
                case LoggerPriority.Error: cmd = LoggerCmd.Error; break;
                case LoggerPriority.Warning: cmd = LoggerCmd.Warn; break;
                default: return;
            }

            var now = bus.Timestamp;
            if ((now - this.lastListenerTime).TotalMilliseconds > Constants.LOGGER_LISTENER_TIMEOUT)
            {
                this.MinPriorityRegister.SetValues(new object[] { (byte)bus.DefaultMinLoggerPriority });
                this.lastListenerTime = TimeSpan.Zero;
            }

            var minPriority = this.MinPriority;
            if (message == null || message.Length == 0 || this.lastListenerTime == TimeSpan.Zero || priority < minPriority)
                return;

            System.Diagnostics.Debug.WriteLine($"{this}: {message}");
            var ev = this.LogEvent;
            if (ev != null)
                ev(this, new LoggerEventArgs(priority, message));

            lock (this)
            {
                try
                {
                    var i = 0;
                    while (i < message.Length)
                    {
                        var chunk = message.Substring(i, Math.Min((int)Constants.JD_SERIAL_MAX_PAYLOAD_SIZE, (int)(message.Length - i)));
                        var payload = PacketEncoding.Pack(LoggerCmdPack.Log, new object[] { chunk });
                        this.SendPacket(Packet.From((ushort)cmd, payload));
                        i += chunk.Length;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
