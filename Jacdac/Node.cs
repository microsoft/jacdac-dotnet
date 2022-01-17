using Jacdac.Servers;
using System;

namespace Jacdac
{
    public delegate void NodeEventHandler(JDNode sender, EventArgs e);



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

        private void RaiseLogEvent(LoggerPriority priority, string message)
        {
            var logger = this.Logger;
            logger?.SendReport(priority, message);
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
