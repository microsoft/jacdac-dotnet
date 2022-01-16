using System;
using System.Diagnostics;
using System.Threading;

namespace Jacdac
{
    /// <summary>
    /// Raised when trying to access a client that is not connected to a server.
    /// </summary>
    [Serializable]
    public sealed class ClientDisconnectedException : Exception
    {
        internal ClientDisconnectedException() { }
    }

    [Serializable]
    public sealed class ClientEventArgs
    {
        readonly object[] Values;
        internal ClientEventArgs(object[] values = null)
        {
            this.Values = values ?? PacketEncoding.Empty;
        }
    }
    public delegate void ClientEventHandler(Client sender, ClientEventArgs args);

    internal sealed class DebouncedClientEventHandler
    {
        public event ClientEventHandler Handler;

        private readonly Client sender;
        private object[] _lastMissedValues = null;
        private Thread _thread;

        public DebouncedClientEventHandler(Client sender)
        {
            this.sender = sender;
        }

        public bool IsEmpty
        {
            get
            {
                return this.Handler == null;
            }
        }
        public void BeginRaise(object[] values)
        {
            var ev = this.Handler;
            if (ev == null) return;

            lock (this.sender)
            {
                if (this._thread != null)
                    this._lastMissedValues = values;
                else
                {
                    this._lastMissedValues = null;
                    this._thread = new Thread(() =>
                    {
                        try
                        {
                            var args = new ClientEventArgs(values);
                            ev.Invoke(this.sender, args);
                        }
                        finally
                        {
                            var missed = this._lastMissedValues;
                            this._thread = null;
                            this._lastMissedValues = null;
                            if (missed != null)
                                this.BeginRaise(missed);
                        }
                    });
                    this._thread.Start();
                }
            }
        }
    }

    public static class ThreadExtensions
    {
        public static void BeginRaiseEvent(object @event, ThreadStart call)
        {
            if (@event == null) return; // optimization shortcut
            Start(call);
        }

        public static void Start(ThreadStart call)
        {
            new Thread(call).Start();
        }
    }

    /// <summary>
    /// A role client which gets bound to a service by the role maanger.
    /// </summary>
    public abstract class Client : JDNode
    {
        private JDBus bus;
        public readonly string Name;
        public readonly uint ServiceClass;
        private JDService _boundService;
        private EventBinding[] events = EventBinding.Empty;
        private RegisterValueBinding[] registerValueBindings = RegisterValueBinding.Empty;

        protected Client(JDBus bus, string name, uint serviceClass)
        {
            this.bus = bus;
            this.Name = name;
            this.ServiceClass = serviceClass;

            var selfDevice = bus.SelfDeviceServer;
            var roleMgr = selfDevice.RoleManager;
            if (roleMgr == null)
                throw new InvalidOperationException("role manager not enabled");

            roleMgr.AddClient(this);
        }

        public override JDBus Bus => this.bus;

        public int CompareTo(Client other)
        {
            return this.Name.CompareTo(other.Name);
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

        /// <summary>
        /// Indicates if the client is connected to a service
        /// </summary>
        public bool IsConnected
        {
            get { return this.BoundService != null; }
        }

        /// <summary>
        /// Gets the bound service
        /// </summary>
        public JDService BoundService
        {
            get { return this._boundService; }
            internal set
            {
                if (this._boundService != value)
                {
                    var old = this._boundService;
                    this.Debug($"{this}: bind {(old?.ToString() ?? "--")} to {(value?.ToString() ?? "--")}");
                    if (old != null)
                    {
                        this._boundService = null;
                        old.Device.Restarted -= this.handleDeviceRestarted;
                        value.Device.Announced -= handleAnnounced;
                        old.EventRaised -= this.handleEventRaised;
                        if (old != null)
                        {
                            ThreadExtensions.BeginRaiseEvent(this.Disconnected, () => this.Disconnected?.Invoke(this, new ServiceEventArgs(old)));
                        }
                    }
                    this._boundService = value;
                    if (value != null)
                    {
                        System.Diagnostics.Debug.Assert(value.ServiceClass == this.ServiceClass);
                        value.Device.Restarted += this.handleDeviceRestarted;
                        value.Device.Announced += handleAnnounced;
                        value.EventRaised += this.handleEventRaised;
                        this.BeginApplyRegisterValueBindings();
                        ThreadExtensions.BeginRaiseEvent(this.Connected, () => this.Connected?.Invoke(this, new ServiceEventArgs(value)));
                    }
                }
            }
        }

        /// <summary>
        /// Raised when a service is bound to the role
        /// </summary>
        public event ServiceEventHandler Connected;
        /// <summary>
        /// Raised when a device reconnects from a restart
        /// </summary>
        public event ServiceEventHandler Configure;
        /// <summary>
        /// Raised when the service is unbound from the role
        /// </summary>
        public event ServiceEventHandler Disconnected;

        public override string ToString()
        {
            return $"{this.Name}<{this.BoundService?.ToString() ?? "?"}";
        }

        /// <summary>
        /// (Optional) Friendly name for the service
        /// </summary>
        public string InstanceName
        {
            get
            {
                return (string)this.GetRegisterValue((ushort)SystemReg.InstanceName, SystemRegPack.InstanceName, new object[] { null });
            }
        }

        public JDService WaitForService(int timeout)
        {
            var s = this.BoundService;
            if (s != null)
                return s;
            if (timeout < 0)
                throw new ClientDisconnectedException();

            ServiceEventHandler signal = null;
            try
            {
                var wait = new AutoResetEvent(false);
                signal = (sender, e) =>
                {
                    this.Connected -= signal;
                    wait.Set();
                };
                this.Connected += signal;
                wait.WaitOne(timeout, true);

                s = this.BoundService;
                if (s == null)
                {
                    this.Log("no service available");
                    throw new ClientDisconnectedException();
                }
                return s;
            }
            catch (Exception)
            {
                throw new ClientDisconnectedException();
            }
            finally
            {
                if (signal != null)
                    this.Connected -= signal;
            }
        }

        protected JDRegister WaitForRegister(ushort code)
        {
            var service = this.WaitForService(Constants.CLIENT_CONNECT_TIMEOUT);
            var reg = service.GetRegister(code);
            return reg;
        }

        protected object[] GetRegisterValues(ushort code, string packFormat, object[] defaultValues = null)
        {
            var reg = this.WaitForRegister(code);
            reg.PackFormat = packFormat;
            var values = reg.WaitForValues(Constants.CLIENT_VALUES_TIMEOUT);
            if (values.Length == 0)
                if (defaultValues != null)
                    return defaultValues;
                else
                {
                    this.Log($"register value of {code} not avaible");
                    throw new ClientDisconnectedException();
                }
            return values;
        }

        protected bool TryGetRegisterValues(ushort code, string packFormat, out object[] values)
        {
            try
            {
                values = this.GetRegisterValues(code, packFormat);
                if (values.Length > 0)
                    return true;
            }
            catch (ClientDisconnectedException)
            { }
            values = null;
            return false;
        }

        protected object GetRegisterValue(ushort code, string packFormat, object[] defaultValues = null)
        {
            var values = this.GetRegisterValues(code, packFormat, defaultValues);
            return values[0];
        }

        protected bool GetRegisterValueAsBool(ushort code, string packFormat, object[] defaultValues = null)
        {
            return (uint)this.GetRegisterValue(code, packFormat, defaultValues) != 0;
        }

        protected void SetRegisterValues(ushort code, string packetFormat, object[] values)
        {
            var data = PacketEncoding.Pack(packetFormat, values);

            // store binding
            RegisterValueBinding rv = null;
            if (!this.TryGetRegisterValueBinding(code, out rv))
                rv = this.AddRegisterValueBinding(code);
            rv.Data = data;

            // update as needed
            this.ApplyRegisterValueBinding(rv);
        }

        private bool TryGetRegisterValueBinding(ushort code, out RegisterValueBinding registerValue)
        {
            var registerValues = this.registerValueBindings;
            foreach (var rv in registerValues)
                if (rv.Code == code)
                {
                    registerValue = rv;
                    return true;
                }

            registerValue = null;
            return false;
        }

        private RegisterValueBinding AddRegisterValueBinding(ushort code)
        {
            lock (this)
            {
                var rvs = this.registerValueBindings;
                RegisterValueBinding registerValue = null;
                if (this.TryGetRegisterValueBinding(code, out registerValue))
                    return registerValue;

                var newRvs = new RegisterValueBinding[rvs.Length + 1];
                rvs.CopyTo(newRvs, 0);
                registerValue = newRvs[rvs.Length] = new RegisterValueBinding(code);
                this.registerValueBindings = newRvs;

                return registerValue;
            }
        }

        private void ApplyRegisterValueBinding(RegisterValueBinding rv)
        {
            var service = this._boundService;
            if (service == null) return;

            var reg = service.GetRegister(rv.Code);
            reg.SendSet(rv.Data);
        }

        private void BeginApplyRegisterValueBindings()
        {
            new Thread(() =>
            {
                var service = this.BoundService;
                if (service == null) return;

                if (this.Configure != null)
                    this.Configure.Invoke(this, new ServiceEventArgs(service));

                var rvs = this.registerValueBindings;
                if (service == null || rvs.Length == 0) return;

                this.Debug($"{this}: apply {rvs.Length} register values");
                foreach (var rv in rvs)
                    this.ApplyRegisterValueBinding(rv);

            }).Start();
        }

        private void handleDeviceRestarted(JDNode sender, EventArgs e)
        {
            this.Debug($"{this}: device {sender} restarted");
            this.BeginApplyRegisterValueBindings();
        }
        private void handleAnnounced(JDNode sender, EventArgs e)
        {
            var service = this.BoundService;
            if (service != null)
            {
                var device = (JDDevice)sender;
                var newService = device.GetService(service.ServiceIndex);
                if (newService != null && newService.ServiceClass == this.ServiceClass)
                    this.BoundService = newService;
                else
                    this.BoundService = null;
            }
        }

        protected void SetRegisterValue(ushort code, string packetFormat, object value)
        {
            this.SetRegisterValues(code, packetFormat, new object[] { value });
        }

        protected void SendCmd(ushort code, byte[] data = null, bool ack = false)
        {
            var service = this.BoundService;
            if (service == null)
            {
                if (ack)
                    throw new ClientDisconnectedException();
                return;
            }
            service.SendPacket(Packet.FromCmd(code, data, ack));
        }

        protected void SendCmdPacked(ushort code, string packFormat, object[] values)
        {
            var payload = PacketEncoding.Pack(packFormat, values);
            this.SendCmd(code, payload);
        }

        private void handleEventRaised(JDService service, EventRaisedArgs args)
        {
            var code = args.Event.Code;
            var events = this.events;
            foreach (var ev in events)
            {
                if (ev.Code == code)
                {
                    ev.DebouncedHandler.BeginRaise(args.Event.Values);
                    break;
                }
            }
        }

        protected void AddEvent(ushort code, ClientEventHandler handler)
        {
            lock (this)
            {
                var events = this.events;
                // don't double add
                foreach (var ev in events)
                    if (ev.Code == code)
                    {
                        ev.DebouncedHandler.Handler += handler;
                        return;
                    }

                var newEvents = new EventBinding[events.Length + 1];
                events.CopyTo(newEvents, 0);
                var newEvent = newEvents[events.Length] = new EventBinding(this, code);
                newEvent.DebouncedHandler.Handler += handler;
                this.events = newEvents;
            }
        }
        protected void RemoveEvent(ushort code, ClientEventHandler handler)
        {
            lock (this)
            {
                for (int i = 0; i < events.Length; i++)
                {
                    var ev = events[i];
                    if (ev.Code == code)
                    {
                        ev.DebouncedHandler.Handler -= handler;
                        if (ev.DebouncedHandler.IsEmpty)
                        {
                            // remove events
                            var newEvents = new EventBinding[events.Length - 1];
                            Array.Copy(events, 0, newEvents, 0, i);
                            if (i + 1 < events.Length)
                                Array.Copy(events, i + 1, newEvents, 0, events.Length - i - 1);
                            this.events = newEvents;
                        }
                        break;
                    }
                }

            }
        }

        sealed class EventBinding
        {
            public static EventBinding[] Empty = new EventBinding[0];

            public readonly ushort Code;
            public readonly DebouncedClientEventHandler DebouncedHandler;

            public EventBinding(Client sender, ushort code)
            {
                this.Code = code;
                this.DebouncedHandler = new DebouncedClientEventHandler(sender);
            }
        }

        sealed class RegisterValueBinding
        {
            public static RegisterValueBinding[] Empty = new RegisterValueBinding[0];

            public readonly ushort Code;
            public byte[] Data;

            public RegisterValueBinding(ushort code)
            {
                this.Code = code;
            }
        }
    }

    public abstract class SensorClient : Client
    {
        private readonly DebouncedClientEventHandler readingChanged;

        protected SensorClient(JDBus bus, string name, uint serviceClass)
            : base(bus, name, serviceClass)
        {
            this.readingChanged = new DebouncedClientEventHandler(this);
            this.Connected += handleConnected;
            this.Disconnected += handleDisconnected;
        }

        /// <summary>
        /// Indicates if the client has received a reading value
        /// </summary>
        public bool HasReadingValue
        {
            get
            {
                var service = this.BoundService;
                var reg = service?.GetRegister((ushort)SystemReg.Reading);
                return reg != null && reg.HasData;
            }
        }

        private void handleConnected(object sender, ServiceEventArgs e)
        {
            var sensor = (SensorClient)sender;
            var service = e.Service;
            var register = service.GetRegister((ushort)SystemReg.Reading);
            register.Changed += handleRegisterChange;
        }

        private void handleDisconnected(object sender, ServiceEventArgs e)
        {
            var sensor = (SensorClient)sender;
            var service = e.Service;
            var register = service.GetRegister((ushort)SystemReg.Reading);
            register.Changed -= handleRegisterChange;
        }

        private void handleRegisterChange(object sender, EventArgs e)
        {
            this.readingChanged.BeginRaise(PacketEncoding.Empty);
        }

        /// <summary>
        /// Raised when the value of the reading changed.
        /// </summary>
        public event ClientEventHandler ReadingChanged
        {
            add { this.readingChanged.Handler += value; }
            remove { this.readingChanged.Handler -= value; }
        }
    }
}
