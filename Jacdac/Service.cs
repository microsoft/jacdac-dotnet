using System;
using System.Collections;

namespace Jacdac
{
    public sealed partial class JDService : JDBusNode
    {
        JDDevice _device;
        private ServiceSpec _specification;
        public readonly byte ServiceIndex;
        public readonly uint ServiceClass;
        JDRegister[] registers;
        JDEvent[] events;

        internal JDService(JDDevice device, byte ServiceIndex, uint ServiceClass)
        {
            this._device = device;
            this.ServiceIndex = ServiceIndex;
            this.ServiceClass = ServiceClass;

            this.registers = new JDRegister[0];
            this.events = new JDEvent[0];
        }

        public static JDService[] EmptyServices = new JDService[0];

        public JDDevice Device
        {
            get { return this._device; }
            internal set { this._device = value; }
        }

        public override JDBus Bus { get => this.Device?.Bus; }

        public override string ToString()
        {
            var device = this.Device;
            var spec = this.Specification;
            var descr = spec == null ? $"0x{this.ServiceClass.ToString("x8")}" : spec.name;
            return device == null ? "?" : $"{device}[{ this.ServiceIndex}:{descr}]";
        }

        /**
         * Gets the cached specification, call ResolveSpecification to make sure that it gets downloaded.
         */
        public ServiceSpec Specification
        {
            get
            {
                if (this._specification == null)
                {
                    var catalog = this.Device?.Bus?.SpecificationCatalog;
                    if (catalog != null)
                    {
                        ServiceSpec spec;
                        if (catalog.TryGetSpecification(this.ServiceClass, out spec))
                            this._specification = spec;
                    }
                }
                return this._specification;
            }
        }

        /**
         * Attempts to resolve the service specification; may trigger a download in a resolver
         */
        public ServiceSpec ResolveSpecification()
        {
            if (this._specification == null)
                this._specification = this.Device?.Bus?.SpecificationCatalog?.ResolveSpecification(this.ServiceClass);
            return this._specification;
        }

        internal void ProcessPacket(Packet pkt)
        {
            //this.emit(PACKET_RECEIVE, pkt);
            if (pkt.IsReport)
            {
                //                this.emit(REPORT_RECEIVE, pkt)
                if (pkt.IsRegisterGet)
                {
                    var reg = this.GetRegister(pkt.RegisterCode);
                    if (null != reg) reg.ProcessPacket(pkt);
                }
                else if (pkt.IsEvent)
                {
                    var ev = this.GetEvent(pkt.EventCode);
                    if (null != ev)
                    {
                        var raised = ev.ProcessPacket(pkt);
                        if (raised)
                            this.EventRaised?.Invoke(this, new EventRaisedArgs(pkt, ev));
                    }
                }
                else if (pkt.ServiceCommand == Jacdac.Constants.CMD_NOT_IMPLEMENTED)
                {
                    var serviceCommand = BitConverter.ToUInt16(pkt.Data, 0);
                    if (
                      serviceCommand >> 12 == Jacdac.Constants.CMD_GET_REG >> 12 ||
                      serviceCommand >> 12 == Jacdac.Constants.CMD_SET_REG >> 12
                  )
                    {
                        var regCode = (ushort)(serviceCommand & Jacdac.Constants.CMD_REG_MASK);
                        JDRegister reg;
                        if (this.TryGetRegister(regCode, out reg))
                            reg.NotImplemented = true;
                    }
                }
                else if (pkt.IsCommand)
                {
                    // this is a report...
                    //console.log("cmd report", { pkt })
                }
            }
            else if (pkt.IsRegisterSet)
            {
                var reg = this.GetRegister(pkt.RegisterCode);
                if (null != reg) reg.ProcessPacket(pkt);
            }
            else if (pkt.IsCommand)
            {
                this.InvalidateRegisterValues(pkt);
                if (this.CommandReceived != null)
                    this.CommandReceived.Invoke(this, new PacketEventArgs(pkt));
            }
        }

        private void InvalidateRegisterValues(Packet pkt)
        {
            var registers = this.registers;
            for (var i = 0; i < registers.Length; i++)
                registers[i].LastGetTimestamp = TimeSpan.Zero;
        }

        public void SendPacket(Packet pkt)
        {
            var device = this.Device;
            if (device == null) return;

            pkt.ServiceIndex = this.ServiceIndex;
            device.SendPacket(pkt);
        }

        public JDRegister[] GetRegisters()
        {
            return this.registers;
        }

        public bool TryGetRegister(ushort code, out JDRegister register)
        {
            var registers = this.registers;
            for (var i = 0; i < registers.Length; ++i)
            {
                var reg = registers[i];
                if (reg.Code == code)
                {
                    register = reg;
                    return true;
                }
            }
            register = null;
            return false;
        }

        public JDRegister GetRegister(ushort code)
        {
            JDRegister r;
            if (!this.TryGetRegister(code, out r))
            {
                lock (this)
                {
                    if (!this.TryGetRegister(code, out r))
                    {
                        r = new JDRegister(this, code);
                        var newRegisters = new JDRegister[this.registers.Length + 1];
                        this.registers.CopyTo(newRegisters, 0);
                        newRegisters[newRegisters.Length - 1] = r;
                        this.registers = newRegisters;
                    }
                }
            }
            return r;
        }

        public JDEvent[] GetEvents()
        {
            return this.events;
        }

        public JDEvent GetEvent(ushort code, bool createIfMissing = false)
        {
            var events = this.events;
            JDEvent r = null;
            for (var i = 0; i < events.Length; ++i)
            {
                var reg = (JDEvent)events[i];
                if (reg.Code == code)
                {
                    r = reg;
                    break;
                }
            }

            if (r == null && createIfMissing)
            {
                lock (this)
                {
                    r = new JDEvent(this, code);
                    var newEvents = new JDEvent[this.events.Length + 1];
                    this.events.CopyTo(newEvents, 0);
                    newEvents[newEvents.Length - 1] = r;
                    this.events = newEvents;
                }
            }
            return r;
        }

        public event EventRaisedHandler EventRaised;

        public event PacketEventHandler CommandReceived;
    }

    [Serializable]
    public sealed class EventRaisedArgs : PacketEventArgs
    {
        public readonly JDEvent Event;
        internal EventRaisedArgs(Packet pkt, JDEvent ev)
            : base(pkt)
        {
            this.Event = ev;
        }
    }

    public delegate void EventRaisedHandler(JDService service, EventRaisedArgs args);

    [Serializable]
    public sealed class ServiceEventArgs : EventArgs
    {
        public readonly JDService Service;
        internal ServiceEventArgs(JDService service)
        {
            this.Service = service;
        }
    }

    public delegate void ServiceEventHandler(JDNode sender, ServiceEventArgs e);
}
