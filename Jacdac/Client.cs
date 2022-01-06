using System;
using System.Threading;

namespace Jacdac
{
    public abstract class Client : JDNode
    {
        public readonly string Name;
        public readonly uint ServiceClass;
        private JDService _boundService;

        public const int TIMEOUT = 2000;

        protected Client(JDBus bus, string name, uint serviceClass)
        {
            this.Name = name;
            this.ServiceClass = serviceClass;

            var selfDevice = bus.SelfDeviceServer;
            var roleMgr = selfDevice.RoleManager;
            if (roleMgr == null)
                throw new InvalidOperationException("role manager not enabled");

            roleMgr.AddClient(this);
        }

        public string Host
        {
            get
            {
                var i = this.Name.IndexOf("/");
                if (i == -1)
                    return this.Name;
                else
                    return this.Name.Substring(0, i);
            }
        }

        public bool IsConnected
        {
            get { return this.BoundService != null; }
        }

        public JDService BoundService
        {
            get { return this._boundService; }
            set
            {
                if (this._boundService != value)
                {
                    this._boundService = value;
                    if (this._boundService != null)
                        this.Connected?.Invoke(this, EventArgs.Empty);
                    else
                        this.Disconnected?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event NodeEventHandler Connected;
        public event NodeEventHandler Disconnected;

        public override string ToString()
        {
            return $"{this.Name}<{this.BoundService?.ToString() ?? "?"}";
        }

        public JDService WaitForService(int timeout)
        {
            var s = this.BoundService;
            if (s != null)
                return s;
            if (timeout < 0)
                throw new ClientDisconnectedException();

            try
            {
                var wait = new AutoResetEvent(false);
                NodeEventHandler signal = null;
                signal = (JDNode node, EventArgs pkt) =>
                {
                    this.Connected -= signal;
                    wait.Set();
                };
                this.Connected += signal;
                wait.WaitOne(timeout, false);

                s = this.BoundService;
                if (s == null)
                    throw new ClientDisconnectedException();
                return s;
            }
            catch (Exception)
            {
                throw new ClientDisconnectedException();
            }
        }

        protected JDRegister WaitForRegister(ushort code, int timeout = TIMEOUT)
        {
            var service = this.WaitForService(timeout);
            var reg = service.GetRegister(code);
            return reg;
        }

        protected object GetRegisterValue(ushort code, string packFormat, object defaultValue = null)
        {
            var reg = this.WaitForRegister(code);
            reg.WaitForData(2000);
            reg.PackFormat = packFormat;
            var values = reg.Values;
            return values.Length == 1 ? values[0] : defaultValue;
        }

        protected void SetRegisterValue(ushort code, string packetFormat, object value)
        {
            var reg = this.WaitForRegister(code);
            reg.SendValues(new object[] { value });
        }

        protected void SendCmd(ushort code, byte[] data = null)
        {
            var service = this.BoundService;
            if (service == null)
                throw new ClientDisconnectedException();
            service.SendPacket(Packet.FromCmd(code));
        }

        protected void SendCmdPacked(ushort code, string packFormat, object[] values)
        {
            var payload = PacketEncoding.Pack(packFormat, values);
            this.SendCmd(code, payload);
        }
    }

    [Serializable]
    public sealed class ClientDisconnectedException : Exception
    {
        public ClientDisconnectedException() { }
    }

    public abstract class SensorClient : Client
    {
        protected SensorClient(JDBus bus, string name, uint serviceClass)
            : base(bus, name, serviceClass)
        { }
    }
}
