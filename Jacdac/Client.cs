using System;
using System.Threading;

namespace Jacdac
{
    [Serializable]
    public sealed class ClientEventArgs : EventArgs
    {
        readonly object[] Values;
        internal ClientEventArgs(object[] values)
        {
            this.Values = values;
        }
    }
    public delegate void ClientEventHandler(Client sender, ClientEventArgs args);

    public abstract class Client : JDNode
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
                    var s = this._boundService;
                    if (s != null)
                        s.EventRaised -= handleEventRaised;
                    this._boundService = value;
                    if (value != null)
                    {
                        value.EventRaised += handleEventRaised;
                        this.Connected?.Invoke(this, EventArgs.Empty);
                    }
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

            NodeEventHandler signal = null;
            try
            {
                var wait = new AutoResetEvent(false);
                signal = (JDNode node, EventArgs pkt) =>
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

        protected object[] GetRegisterValues(ushort code, string packFormat)
        {
            var reg = this.WaitForRegister(code);
            reg.PackFormat = packFormat;
            var values = reg.WaitForValues(VALUES_TIMEOUT);
            if (values.Length == 0)
                throw new ClientDisconnectedException();
            return values;
        }

        protected object GetRegisterValue(ushort code, string packFormat)
        {
            return this.GetRegisterValues(code, packFormat)[0];
        }

        protected void SetRegisterValues(ushort code, string packetFormat, object[] values)
        {
            var reg = this.WaitForRegister(code);
            reg.SendValues(values);
        }

        protected void SetRegisterValue(ushort code, string packetFormat, object value)
        {
            this.SetRegisterValues(code, packetFormat, new object[] { value });
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

        private void handleEventRaised(JDService service, EventRaisedArgs args)
        {
            var code = args.Event.Code;
            var pkt = args.Packet;
            var events = this.events;
            ClientEventArgs eargs = null;
            foreach (var ev in events)
            {
                if (ev.Code == code)
                {
                    if (eargs == null)
                        eargs = new ClientEventArgs(args.Event.Values);
                    ev.Handler(this, eargs);
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
                    if (ev.Code == code && ev.Handler == handler)
                        return;

                var newEvents = new EventBinding[events.Length + 1];
                events.CopyTo(newEvents, 0);
                newEvents[events.Length] = new EventBinding(code, handler);
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
                    if (ev.Code == code && ev.Handler == handler)
                    {
                        // remove events
                        var newEvents = new EventBinding[events.Length - 1];
                        Array.Copy(events, 0, newEvents, 0, i);
                        if (i + 1 < events.Length)
                            Array.Copy(events, i + 1, newEvents, 0, events.Length - i - 1);
                        this.events = newEvents;
                        break;
                    }
                }

            }
        }

        struct EventBinding
        {
            public static EventBinding[] Empty = new EventBinding[0];
            public readonly ushort Code;
            public readonly ClientEventHandler Handler;
            public EventBinding(ushort code, ClientEventHandler handler)
            {
                this.Code = code;
                this.Handler = handler;
            }
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
