using System;
using System.Collections;

namespace Jacdac
{
    public sealed class JDService : JDNode
    {
        JDDevice _device;
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

        public JDDevice Device
        {
            get { return this._device; }
            internal set { this._device = value; }
        }

        public override string ToString()
        {
            return $"{this.Device}[{this.ServiceIndex}]";
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
                    if (null != ev) ev.ProcessPacket(pkt);
                }
                else if (pkt.ServiceCommand == Jacdac.Constants.CMD_NOT_IMPLEMENTED)
                {
                    var serviceCommand = Util.Read16(pkt.Data, 0);
                    if (
                      serviceCommand >> 12 == Jacdac.Constants.CMD_GET_REG >> 12 ||
                      serviceCommand >> 12 == Jacdac.Constants.CMD_SET_REG >> 12
                  )
                    {
                        var regCode = (ushort)(serviceCommand & Jacdac.Constants.CMD_REG_MASK);
                        var reg = this.GetRegister(regCode, false);
                        if (null != reg) reg.NotImplemented = true;
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
            pkt.ServiceIndex = this.ServiceIndex;
            this.Device.SendPacket(pkt);
        }

        public JDRegister[] GetRegisters()
        {
            return (JDRegister[])this.registers.Clone();
        }

        public JDRegister GetRegister(ushort code, bool createIfMissing = false)
        {
            var registers = this.registers;
            JDRegister r = null;
            for (var i = 0; i < registers.Length; ++i)
            {
                var reg = registers[i];
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
                    r = new JDRegister(this, code);
                    var newRegisters = new JDRegister[this.registers.Length + 1];
                    this.registers.CopyTo(newRegisters, 0);
                    newRegisters[newRegisters.Length - 1] = r;
                    this.registers = newRegisters;
                }
            }
            return r;
        }

        public JDEvent[] GetEvents()
        {
            return (JDEvent[])this.events.Clone();
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

        public event PacketEventHandler CommandReceived;
    }
}
