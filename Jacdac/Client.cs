using System;
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
    public sealed class ClientEventArgs : EventArgs
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

    public abstract class Client
    {
        public readonly string Name;
        public readonly uint ServiceClass;
        private JDService _boundService;
        private EventBinding[] events = EventBinding.Empty;

        public static int CONNECT_TIMEOUT = 3000;
        public static int VALUES_TIMEOUT = 1000;

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
                    var old = this._boundService;
                    if (old != null)
                    {
                        this._boundService = null;
                        old.EventRaised -= this.handleEventRaised;
                        if (old != null)
                            ThreadExtensions.BeginRaiseEvent(this.Disconnected, () => this.Disconnected?.Invoke(this, new ServiceEventArgs(old)));
                    }
                    this._boundService = value;
                    if (value != null)
                    {
                        value.EventRaised += this.handleEventRaised;
                        ThreadExtensions.BeginRaiseEvent(this.Connected, () => this.Connected?.Invoke(this, new ServiceEventArgs(value)));
                    }
                }
            }
        }

        public event ServiceEventHandler Connected;
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
                    throw new ClientDisconnectedException();
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
            var service = this.WaitForService(CONNECT_TIMEOUT);
            var reg = service.GetRegister(code);
            return reg;
        }

        protected object[] GetRegisterValues(ushort code, string packFormat, object[] defaultValues = null)
        {
            var reg = this.WaitForRegister(code);
            reg.PackFormat = packFormat;
            var values = reg.WaitForValues(VALUES_TIMEOUT);
            if (values.Length == 0)
                if (defaultValues != null)
                    return defaultValues;
                else
                    throw new ClientDisconnectedException();
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
            var reg = this.WaitForRegister(code);
            reg.PackFormat = packetFormat;
            reg.SendValues(values);
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
            var events = this.events;
            lock (events)
            {
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
            var events = this.events;
            lock (events)
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
