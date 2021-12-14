using System;

namespace Jacdac
{
    public abstract class JDServer : JDNode
    {
        public byte ServiceIndex;
        public JDBus Bus;
        public readonly uint ServiceClass;
        private JDRegisterServer[] registers;

        public JDServer(uint serviceClass)
        {
            this.ServiceClass = serviceClass;
            this.registers = new JDRegisterServer[0];
        }

        public virtual void ProcessPacket(Packet pkt)
        {
            if (pkt.IsRegisterGet || pkt.IsRegisterSet)
            {
                JDRegisterServer register;
                if (this.TryGetRegister(pkt.RegisterCode, out register))
                    register.ProcessPacket(pkt);
                return;
            }
        }

        public void AddRegister(JDRegisterServer register)
        {
            lock (this)
            {
                register.Server = this;

                var registers = this.registers;
                var newRegisters = new JDRegisterServer[registers.Length + 1];
                registers.CopyTo(newRegisters, 0);
                newRegisters[newRegisters.Length - 1] = register;
                this.registers = newRegisters;
            }
        }

        public bool TryGetRegister(ushort code, out JDRegisterServer register)
        {
            var registers = this.registers;
            for (var i = 0; i < registers.Length; i++)
            {
                var r = registers[i];
                if (r.Code == code)
                {
                    register = r;
                    return true;
                }
            }
            register = null;
            return false;
        }

        public void SendPacket(Packet pkt)
        {
            var bus = this.Bus;
            if (bus == null) return;

            pkt.DeviceId = bus.SelfDevice.DeviceId;
            pkt.ServiceIndex = this.ServiceIndex;
            bus.SendPacket(pkt);
        }
    }

    public abstract class JDRegisterServer : JDNode
    {
        public JDServer Server;
        public readonly ushort Code;
        public JDRegisterServer(ushort code)
        {
            this.Code = code;
        }
        public abstract void ProcessPacket(Packet pkt);
    }


    public sealed class JDStaticRegisterServer : JDRegisterServer
    {
        public readonly string Format;
        public byte[] Data;
        public TimeSpan LastSetTime;

        public JDStaticRegisterServer(ushort code, string format, object[] value)
            : base(code)
        {
            this.Format = format;
            this.Data = new byte[0];
            this.LastSetTime = TimeSpan.Zero;
            this.SetValue(value);
        }

        public void SetValue(object[] value)
        {
            var packed = PacketEncoding.Pack(this.Format, value);
            if (!Util.BufferEquals(this.Data, packed))
            {
                this.Data = packed;
                this.RaiseChanged();
            }
        }

        public object[] GetValue()
        {
            if (this.Data.Length == 0) return new object[0];
            return PacketEncoding.UnPack(this.Format, this.Data);
        }

        public void SendGet()
        {
            var server = this.Server;
            if (server == null) return;

            var pkt = Packet.From((ushort)(Jacdac.Constants.CMD_GET_REG | this.Code), this.Data);
            this.Server.SendPacket(pkt);
        }

        public override void ProcessPacket(Packet pkt)
        {
            if (pkt.IsRegisterGet)
                this.SendGet();
            else if (pkt.IsRegisterSet)
                this.SetData(pkt);
        }

        private void SetData(Packet pkt)
        {
            try
            {
                this.LastSetTime = pkt.Timestamp;
                var values = PacketEncoding.UnPack(this.Format, pkt.Data);
                this.SetValue(values);
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("invalid format");
            }
        }
    }

    public sealed class ControlServer : JDServer
    {
        public readonly JDBusOptions Options;

        internal ControlServer(JDBusOptions options)
            : base(Jacdac.ControlConstants.ServiceClass)
        {
            this.Options = options;
            if (this.Options?.FirmwareVersion != null)
                this.AddRegister(new JDStaticRegisterServer((ushort)Jacdac.ControlReg.FirmwareVersion, "s", new object[] { this.Options.FirmwareVersion }));
            if (this.Options?.Description != null)
                this.AddRegister(new JDStaticRegisterServer((ushort)Jacdac.ControlReg.DeviceDescription, "s", new object[] { this.Options.Description }));
        }
    }
}
