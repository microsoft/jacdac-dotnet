using Jacdac.Servers;
using System;

namespace Jacdac
{
    public delegate void NodeEventHandler(JDNode sender, EventArgs e);

    public abstract class JDNode
    {
        internal JDNode()
        {

        }

        public event NodeEventHandler Changed;

        protected void RaiseChanged()
        {
            this.Changed?.Invoke(this, EventArgs.Empty);
        }

        public bool HasChangedListeners()
        {
            return this.Changed != null;
        }
    }

    public abstract class JDLoggerNode : JDNode
    {
        public abstract LoggerServer Logger { get; }

        protected JDLoggerNode() { }

        protected void Debug(string msg)
        {
            System.Diagnostics.Debug.WriteLine($"{this}: {msg}");
            var logger = this.Logger;
            logger?.SendReport(LoggerPriority.Debug, msg);
        }

        protected void Warn(string msg)
        {
            var logger = this.Logger;
            logger?.SendReport(LoggerPriority.Warning, msg);
        }

        protected void Log(string msg)
        {
            var logger = this.Logger;
            logger?.SendReport(LoggerPriority.Log, msg);
        }

        protected void Error(string msg)
        {
            var logger = this.Logger;
            logger?.SendReport(LoggerPriority.Error, msg);
        }
    }

    public abstract class JDBusNode : JDLoggerNode
    {
        protected JDBusNode()
        {

        }

        public abstract JDBus Bus { get; }

        public override LoggerServer Logger { get => this.Bus?.Logger; }
    }


    public abstract class JDServiceNode : JDBusNode
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
