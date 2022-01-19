using System;

namespace Jacdac
{
    internal sealed class JDCommand
    {
        public readonly ushort Code;
        public readonly PacketEventHandler Handler;
        public JDCommand(ushort code, PacketEventHandler handler)
        {
            this.Code = code;
            this.Handler = handler;
        }
    }

    public class JDServiceServerOptions
    {
        public string InstanceName;
    }

    public abstract partial class JDServiceServer : JDNode, IDisposable
    {
        public byte ServiceIndex;
        public JDDeviceServer Device;
        public readonly uint ServiceClass;
        private JDRegisterServer[] registers;
        private JDCommand[] commands;

        protected JDStaticRegisterServer statusCodeRegister;

        protected JDServiceServer(uint serviceClass, JDServiceServerOptions options)
        {
            this.ServiceClass = serviceClass;
            this.registers = new JDRegisterServer[0];
            this.commands = new JDCommand[0];

            this.AddRegister(this.statusCodeRegister = new JDStaticRegisterServer((ushort)SystemReg.StatusCode, SystemRegPack.StatusCode, new object[] { 0, 0 }));
            this.statusCodeRegister.Changed += handleStatusCodeChanged;

            if (!String.IsNullOrEmpty(options?.InstanceName))
                this.AddRegister(new JDStaticRegisterServer((ushort)Jacdac.BaseReg.InstanceName, Jacdac.BaseRegPack.InstanceName, new object[] { options.InstanceName })
                {
                    IsConst = true
                });
        }

        public ushort ServiceStatusCode
        {
            get { return (ushort)this.statusCodeRegister.GetValues()[0]; }
            set
            {
                var values = this.statusCodeRegister.GetValues();
                values[0] = value;
                this.statusCodeRegister.SetValues(values);
            }
        }
        public ushort VendorStatusCode
        {
            get { return (ushort)this.statusCodeRegister.GetValues()[1]; }
            set
            {
                var values = this.statusCodeRegister.GetValues();
                values[1] = value;
                this.statusCodeRegister.SetValues(values);
            }
        }

        private void handleStatusCodeChanged(JDNode sender, EventArgs e)
        {
            this.SendEvent((ushort)SystemEvent.StatusCodeChanged, this.statusCodeRegister.Data);
        }

        public override JDBus Bus { get => this.Device?.Bus; }

        public virtual bool ProcessPacket(Packet pkt)
        {
            if (pkt.IsRegisterGet || pkt.IsRegisterSet)
            {
                JDRegisterServer register;
                if (this.TryGetRegister(pkt.RegisterCode, out register))
                {
                    var res = register.ProcessPacket(pkt);
                    if (res) return true;
                }
            }
            else if (pkt.IsCommand)
            {
                PacketEventHandler handler;
                if (this.TryGetCommand(pkt.ServiceCommand, out handler))
                {
                    try
                    {
                        handler(this, new PacketEventArgs(pkt));
                    }
                    catch (Exception ex)
                    {
                        var logger = this.Device?.Bus?.Logger;
                        if (logger != null)
                            logger.SendReport(LoggerPriority.Error, $"cmd failed: {ex.Message}, {pkt}");
                    }
                    return true;
                }
            }

            if (!pkt.IsMultiCommand)
                this.SendNotImplemented(pkt);

            return false;
        }

        protected void SendNotImplemented(Packet pkt)
        {
            System.Diagnostics.Debug.Assert(!pkt.IsMultiCommand);
            var data = new byte[4];
            // Jacdac.SystemCmdPack.CommandNotImplemented
            Util.Write16(data, 0, pkt.ServiceCommand);
            Util.Write16(data, 2, pkt.Crc);
            var resp = Packet.FromCmd((ushort)Jacdac.BaseCmd.CommandNotImplemented, data);
            this.SendPacket(resp);
        }

        public void AddRegister(JDRegisterServer register)
        {
            lock (this)
            {
                register.Service = this;

                var registers = this.registers;
                var newRegisters = new JDRegisterServer[registers.Length + 1];
                registers.CopyTo(newRegisters, 0);
                newRegisters[newRegisters.Length - 1] = register;
                this.registers = newRegisters;
            }
        }

        public JDStaticRegisterServer AddRegister(ushort code, string packFormat, object[] values)
        {
            var register = new JDStaticRegisterServer(code, packFormat, values);
            this.AddRegister(register);
            return register;
        }

        public void AddCommand(ushort code, PacketEventHandler handler)
        {
            lock (this)
            {
                var commands = this.commands;
                var newCommands = new JDCommand[commands.Length + 1];
                commands.CopyTo(newCommands, 0);
                newCommands[newCommands.Length - 1] = new JDCommand(code, handler);
                this.commands = newCommands;
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

        public bool TryGetCommand(ushort code, out PacketEventHandler command)
        {
            var commands = this.commands;
            for (var i = 0; i < commands.Length; i++)
            {
                var r = commands[i];
                if (r.Code == code)
                {
                    command = r.Handler;
                    return true;
                }
            }
            command = null;
            return false;
        }

        public void SendPacket(Packet pkt)
        {
            var device = this.Device;
            if (device == null) return;

            pkt.ServiceIndex = this.ServiceIndex;
            pkt.DeviceId = device.DeviceId;
            device.SendPacket(pkt);
        }

        public void SendEvent(ushort eventCode, byte[] data = null)
        {
            var device = this.Device;
            if (device == null) return;

            var now = device.Bus.Timestamp;
            var cmd = device.CreateEventCmd(eventCode);
            var pkt = Packet.From(cmd, data);
            pkt.ServiceIndex = this.ServiceIndex;
            pkt.DeviceId = device.DeviceId;
            this.SendPacket(pkt);
            device.DelayedSendPacket(pkt, (int)now.TotalMilliseconds + 20);
            device.DelayedSendPacket(pkt, (int)now.TotalMilliseconds + 100);
        }

        public virtual void Dispose()
        {

        }
    }
}
