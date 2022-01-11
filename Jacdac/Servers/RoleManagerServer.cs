using System;
using System.Diagnostics;

namespace Jacdac.Servers
{
    public sealed partial class RoleManagerServer : JDServiceServer
    {
        private readonly ISettingsStorage Storage;
        private JDStaticRegisterServer autoBindRegister;
        private JDStaticRegisterServer allRolesAllocatedRegister;

        private Client[] roles = new Client[0];

        public RoleManagerServer(ISettingsStorage storage = null)
            : base(ServiceClasses.RoleManager, null)
        {
            this.Storage = storage;
            this.AddRegister(this.autoBindRegister = new JDStaticRegisterServer((ushort)Jacdac.RoleManagerReg.AutoBind, Jacdac.RoleManagerRegPack.AutoBind, new object[] { 1 }));
            this.AddRegister(this.allRolesAllocatedRegister = new JDStaticRegisterServer((ushort)Jacdac.RoleManagerReg.AllRolesAllocated, Jacdac.RoleManagerRegPack.AllRolesAllocated, new object[] { false }));
            this.AddCommand((ushort)Jacdac.RoleManagerCmd.SetRole, this.handleSetRole);
            this.AddCommand((ushort)Jacdac.RoleManagerCmd.ListRoles, this.handleListRoles);
            this.AddCommand((ushort)Jacdac.RoleManagerCmd.ClearAllRoles, this.handleClearAllRoles);

            this.Changed += this.handleChanged;
            this.allRolesAllocatedRegister.Changed += handleAllRolesAllocatedChanged;
        }

        public bool AutoBind
        {
            get { return this.autoBindRegister.GetValueAsBool(); }
        }

        /// <summary>
        /// Raised when all roles are connected
        /// </summary>
        public event NodeEventHandler Connected;

        /// <summary>
        /// Raised when all roles are disconnected
        /// </summary>
        public event NodeEventHandler Disconnected;

        public Client[] Roles
        {
            get { return this.Roles; }
        }

        private void handleAllRolesAllocatedChanged(JDNode sender, EventArgs e)
        {
            var v = this.allRolesAllocatedRegister.GetValueAsBool();
            if (v)
                this.Connected?.Invoke(this, e);
            else
                this.Disconnected?.Invoke(this, e);
        }

        public void AddClient(Client role)
        {
            lock (this.roles)
            {
                Client binding;
                if (this.TryGetClient(role.Name, out binding) && binding != role)
                    throw new ArgumentException("role already allocated");

                var roles = this.roles;
                var newRoles = new Client[roles.Length + 1];
                roles.CopyTo(newRoles, 0);
                newRoles[roles.Length] = role;

                this.roles = newRoles;
            }
            this.RaiseChanged();
        }

        private void handleChanged(JDNode sender, EventArgs e)
        {
            // recompute all roles allocated
            var allRolesAllocated = true;
            var bindings = this.roles;
            foreach (var binding in bindings)
                if (binding.BoundService == null)
                {
                    allRolesAllocated = false;
                    break;
                }
            this.SaveBindings();
            this.allRolesAllocatedRegister.SetValues(new object[] { allRolesAllocated });
            this.SendEvent((ushort)Jacdac.RoleManagerEvent.Change);
        }

        const string PACK_FORMAT = "b[8] u32 u8";
        private void SaveBindings()
        {
            if (this.Storage == null) return;

            var roles = this.roles;
            foreach (var role in roles)
            {
                var service = role.BoundService;
                var device = service?.Device;
                if (device != null)
                {
                    var current = this.Storage.Read(role.Name);
                    var did = HexEncoding.ToBuffer(device.DeviceId);
                    var payload = PacketEncoding.Pack(PACK_FORMAT, new object[] {
                        did,
                        role.ServiceClass,
                        service.ServiceIndex
                    });
                    if (!Util.BufferEquals(current, payload))
                    {
                        this.Debug($"roles: save {role}");
                        this.Storage.Write(role.Name, payload);
                    }
                }
            }
        }

        private bool TryLoadBinding(JDDevice[] devices, Client role)
        {
            if (this.Storage == null)
                return false;

            try
            {
                var binding = this.Storage.Read(role.Name);
                if (binding == null || binding.Length == 0)
                    return false;
                var data = PacketEncoding.UnPack(PACK_FORMAT, binding);
                var did = (byte[])data[0];
                var dids = HexEncoding.ToString(did);
                var sid = (uint)data[1];
                var idx = (uint)data[2];
                if (sid != role.ServiceClass)
                {
                    // invalid binding
                    this.Storage.Delete(role.Name);
                    return false;
                }

                foreach (var device in devices)
                {
                    if (device.DeviceId == dids
                    && idx < device.GetServices().Length)
                    {
                        role.BoundService = device.GetService(idx);
                        break;
                    }
                }
                return role.BoundService != null;
            }
            catch (Exception)
            {
                this.Storage.Delete(role.Name);
                return false;
            }
        }

        private void handleClearAllRoles(JDNode sensor, PacketEventArgs args)
        {
            this.Debug($"roles: clear");
            if (this.Storage != null)
                this.Storage.Clear();
            var bindings = this.roles;
            foreach (var binding in bindings)
                binding.BoundService = null;
            this.RaiseChanged();
        }

        private void handleListRoles(JDNode sensor, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var pipe = OutPipe.From(this.Device.Bus, pkt);
            var roles = this.roles;
            pipe?.RespondForEach(roles, k =>
            {
                var binding = (Client)k;
                var service = binding.BoundService;
                var serviceIndex = service == null ? 0 : service.ServiceIndex;
                var device = service?.Device;
                var did = device == null ? new byte[0] : HexEncoding.ToBuffer(device.DeviceId);
                return PacketEncoding.Pack("b[8] u32 u8 s", new object[] { did, binding.ServiceClass, serviceIndex, binding.Name });
            });
        }

        private void handleSetRole(JDNode sensor, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var values = PacketEncoding.UnPack(Jacdac.RoleManagerCmdPack.SetRole, pkt.Data);
            var did = (byte[])values[0];
            var deviceId = HexEncoding.ToString(did);
            var serviceIndex = (uint)values[1];
            var name = (string)values[2];

            lock (this.roles)
            {
                Client role;
                if (!this.TryGetClient(name, out role))
                    return; // role does not exist

                var service = role.BoundService;
                if (service != null && service.Device?.DeviceId == deviceId && service.ServiceIndex == serviceIndex)
                    return; // already bound

                JDDevice device;
                if (!this.Device.Bus.TryGetDevice(deviceId, out device))
                    return; // device does not exist

                role.BoundService = device.GetService(serviceIndex);
            }
            this.RaiseChanged();
        }

        private bool TryGetClient(string role, out Client binding)
        {
            var bindings = this.roles;
            foreach (var b in bindings)
                if (b.Name == role)
                {
                    binding = b;
                    return true;
                }
            binding = null;
            return false;
        }

        private string ComputeHash()
        {
            string s = "";
            var roles = this.roles;
            foreach (var role in roles)
                s += ";" + role.ToString();
            return s;
        }

        private bool TryGetClient(JDService service, out Client client)
        {
            var roles = this.roles;
            foreach (var role in roles)
            {
                if (role.BoundService == service)
                {
                    client = role;
                    return true;
                }
            }
            client = null;
            return false;
        }

        public void BindRoles()
        {
            lock (this.roles)
                this.SyncBindRoles();
        }

        private void SyncBindRoles()
        {
            var hash = this.ComputeHash();
            var bus = this.Device.Bus;
            var devices = bus.GetDevices();
            var roles = this.roles;
            var bound = 0;
            // check that services are still current
            foreach (var role in roles)
            {
                var service = role.BoundService;
                if (service?.Device?.Bus == null)
                    role.BoundService = null;
                if (role.BoundService != null)
                    bound++;
            }

            // nothing to do here
            if (bound == roles.Length)
                return;


            this.Debug($"roles: binding {bound}/{roles.Length}");
            // try to load bindings from storage
            if (this.Storage != null)
            {
                foreach (var role in roles)
                {
                    // already bound?
                    if (role.BoundService != null) continue;

                    // try get binding from storage
                    if (this.TryLoadBinding(devices, role))
                    {
                        this.Debug($"roles: storage bind {role}");
                        continue;
                    }
                }
            }

            // dynamic allocation
            if (this.autoBindRegister.GetValueAsBool())
            {
                foreach (var role in roles)
                {
                    // already bound?
                    if (role.BoundService != null) continue;

                    // find candidates
                    foreach (var device in devices)
                    {
                        var services = device.GetServices();
                        foreach (var service in services)
                        {
                            Client client;
                            // serice class match
                            if (service.ServiceClass == role.ServiceClass
                                // not bound yet
                                && !this.TryGetClient(service, out client))
                            {
                                role.BoundService = service;
                                this.Debug($"roles: auto bind {role}");
                                break;
                            }
                        }
                        // found a bound
                        if (role.BoundService != null)
                            break;
                    }
                }
            }

            if (hash != this.ComputeHash())
                this.RaiseChanged();
        }
    }
}

