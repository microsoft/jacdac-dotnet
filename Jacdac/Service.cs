using System;
using System.Collections;

namespace Jacdac
{
    public sealed class JDService : JDNode
    {
        JDDevice _device;
        public readonly int ServiceIndex;
        public readonly uint ServiceClass;
        ArrayList _registers;
        ArrayList _events;

        internal JDService(JDDevice device, int ServiceIndex, uint ServiceClass)
        {
            this._device = device;
            this.ServiceIndex = ServiceIndex;
            this.ServiceClass = ServiceClass;

            this._registers = new ArrayList();
            this._events = new ArrayList();
        }

        public JDDevice Device
        {
            get { return this._device; }
            internal set { this._device = value; }
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
                else if (pkt.ServiceCommand == (ushort)Jacdac.SystemCmd.CommandNotImplemented)
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
                //  this.emit(COMMAND_RECEIVE, pkt)
            }
        }

        private void InvalidateRegisterValues(Packet pkt)
        {
            var regs = this.Registers();
            for (var i = 0; i < regs.Length; i++)
                regs[i].LastGetTimestamp = TimeSpan.Zero;
        }

        public JDRegister[] Registers()
        {
            lock (this)
                return this._registers.ToArray() as JDRegister[];
        }

        public JDRegister GetRegister(ushort code, bool createIfMissing = false)
        {
            lock (this)
            {
                JDRegister r = null;
                for (var i = 0; i < this._registers.Count; ++i)
                {
                    var reg = (JDRegister)this._registers[i];
                    if (reg.Code == code)
                    {
                        r = reg;
                        break;
                    }
                }

                if (r == null && createIfMissing)
                {
                    r = new JDRegister(this, code);
                    this._registers.Add(r);
                }
                return r;
            }
        }

        public JDEvent[] Events()
        {
            lock (this)
                return this._events.ToArray() as JDEvent[];
        }

        public JDEvent GetEvent(ushort code, bool createIfMissing = false)
        {
            lock (this)
            {
                JDEvent r = null;
                for (var i = 0; i < this._events.Count; ++i)
                {
                    var reg = (JDEvent)this._events[i];
                    if (reg.Code == code)
                    {
                        r = reg;
                        break;
                    }
                }

                if (r == null && createIfMissing)
                {
                    r = new JDEvent(this, code);
                    this._events.Add(r);
                }
                return r;
            }
        }
    }
}
