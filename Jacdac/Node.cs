using Jacdac.Servers;
using System;

namespace Jacdac
{
    public delegate void NodeEventHandler(JDNode sender, EventArgs e);

    public delegate void LoggerEventHandler(JDNode sender, LoggerEventArgs e);

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

    public abstract class JDNode
    {
        internal protected JDNode()
        {

        }

        /// <summary>
        /// Gets the instance of the bus, if any
        /// </summary>
        public abstract JDBus Bus { get; }

        /// <summary>
        /// Raised when the state of the node is changed
        /// </summary>
        public event NodeEventHandler Changed;

        protected void RaiseChanged()
        {
            this.Changed?.Invoke(this, EventArgs.Empty);
        }

        public bool HasChangedListeners()
        {
            return this.Changed != null;
        }

        /// <summary>
        /// Gets the instance to the loggger server, if any
        /// </summary>
        public virtual LoggerServer Logger => this.Bus?.Logger;

        /// <summary>
        /// Raised when a new log event is added locally
        /// </summary>
        public event LoggerEventHandler LogEvent;

        private void RaiseLogEvent(LoggerPriority priority, string message)
        {
            var logger = this.Logger;
            LoggerPriority minPriority = logger != null ? logger.MinPriority : LoggerPriority.Warning;
            if (minPriority <= priority)
                System.Diagnostics.Debug.WriteLine($"{this}: {message}");
            if (logger != null)
                logger?.SendReport(priority, message);
            var ev = this.LogEvent;
            if (ev != null)
                ev(this, new LoggerEventArgs(priority, message));
        }

        protected void Debug(string msg)
        {
            this.RaiseLogEvent(LoggerPriority.Debug, msg);
        }

        protected void Warn(string msg)
        {
            this.RaiseLogEvent(LoggerPriority.Warning, msg);
        }

        protected void Log(string msg)
        {
            this.RaiseLogEvent(LoggerPriority.Log, msg);
        }

        protected void Error(string msg)
        {
            this.RaiseLogEvent(LoggerPriority.Error, msg);
        }
    }

    public abstract class JDServiceNode : JDNode
    {
        public readonly JDService Service;
        public readonly ushort Code;

        internal JDServiceNode(JDService service, ushort code)
        {
            this.Service = service;
            this.Code = code;
        }

        public override JDBus Bus
        {
            get { return this.Service.Device?.Bus; }
        }
    }
}
