using Jacdac.Servers;
using System;

namespace Jacdac
{
    public delegate void NodeEventHandler(JDNode sender, EventArgs e);

    public abstract partial class JDNode
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

        protected void LogDebug(string msg)
        {
            Platform.LogDebug($"{this}: {msg}", "Jacdac");
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
