using System;

namespace Jacdac
{
    public abstract class Client : JDNode
    {
        public readonly string Name;
        public readonly uint ServiceClass;
        private JDService _boundService;

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

        protected JDRegister GetRegister(ushort code)
        {
            var reg = this.BoundService?.GetRegister(code);
            reg?.RefreshMaybe();
            return reg;
        }

        public event NodeEventHandler Connected;
        public event NodeEventHandler Disconnected;

        public override string ToString()
        {
            return $"{this.Name}<{this.BoundService?.ToString() ?? "?"}";
        }

        protected object GetRegisterValue(ushort code, string packFormat, object defaultValue = null)
        {
            var reg = this.GetRegister(code);
            var value = reg?.Value(packFormat);
            return value != null ? value : defaultValue;
        }
    }

    public abstract class SensorClient : Client
    {
        protected SensorClient(JDBus bus, string name, uint serviceClass)
            : base(bus, name, serviceClass)
        { }
    }
}
