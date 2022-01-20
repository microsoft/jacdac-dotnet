using System;
using System.Diagnostics;

namespace Jacdac.Servers
{
    public sealed partial class RoleManagerServer : JDServiceServer
    {
        private readonly ISettingsStorage Storage;
        private JDStaticRegisterServer autoBindRegister;
        private JDStaticRegisterServer allRolesAllocatedRegister;
        private bool binding = false;

        private Client[] roles = new Client[0];
        public TimeSpan LastBindRoles { get; private set; } = TimeSpan.Zero;

        public RoleManagerServer(ISettingsStorage storage = null)
            : base(ServiceClasses.RoleManager, null)
        {
            this.Storage = storage;
            this.AddRegister(this.autoBindRegister = new JDStaticRegisterServer((ushort)Jacdac.RoleManagerReg.AutoBind, Jacdac.RoleManagerRegPack.AutoBind, new object[] { 1 }));
            this.AddRegister(this.allRolesAllocatedRegister = new JDStaticRegisterServer((ushort)Jacdac.RoleManagerReg.AllRolesAllocated, Jacdac.RoleManagerRegPack.AllRolesAllocated, new object[] { false }));
            this.AddCommand((ushort)RoleManagerCmd.SetRole, this.handleSetRole);
            this.AddCommand((ushort)RoleManagerCmd.ListRoles, this.handleListRoles);
            this.AddCommand((ushort)RoleManagerCmd.ClearAllRoles, this.handleClearAllRoles);

            this.Changed += this.handleChanged;
            this.allRolesAllocatedRegister.Changed += handleAllRolesAllocatedChanged;
        }

        public override string ToString()
        {
            return "roles";
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
            get { return this.roles; }
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

                // insert client sorted by name
                var i = 0;
                for (i = 0; i < roles.Length; i++)
                {
                    if (role.CompareTo(roles[i]) < 0)
                        break;
                }
                if (i > 0)
                    Array.Copy(roles, 0, newRoles, 0, i);
                newRoles[i] = role;
                if (i < roles.Length)
                    Array.Copy(roles, i, newRoles, i + 1, roles.Length - i);

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
            this.LogDebug("changed");
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
                        this.LogDebug($"save {role}");
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
                        var service = device.GetService(idx);
                        Client existingClient;
                        if (!this.TryGetClient(service, out existingClient))
                        {
                            role.BoundService = service;
                            break;
                        }
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
            this.ClearRoles();
        }

        /// <summary>
        /// Clears all role information
        /// </summary>
        public void ClearRoles()
        {
            this.LogDebug($"clear");
            if (this.Storage != null)
                this.Storage.Clear();
            var bindings = this.roles;
            foreach (var binding in bindings)
                binding.BoundService = null;
            this.RaiseChanged();
        }

        private void handleListRoles(JDNode sensor, PacketEventArgs args)
        {
            var bus = this.Bus;
            var pkt = args.Packet;
            var pipe = OutPipe.From(bus, pkt);
            if (pipe == null) return;
            var roles = this.roles;
            pipe.RespondForEach(roles, k =>
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

            this.BindRole(name, deviceId, serviceIndex);
        }

        /// <summary>
        /// Binds a role to a service
        /// </summary>
        /// <param name="name"></param>
        /// <param name="deviceId"></param>
        /// <param name="serviceIndex"></param>
        public void BindRole(string name, string deviceId, uint serviceIndex)
        {
            var did = JDDevice.ShortDeviceId(deviceId);
            lock (this.roles)
            {
                Client role;
                if (!this.TryGetClient(name, out role))
                    return; // role does not exist

                var oldService = role.BoundService;
                if (oldService != null && oldService.Device?.DeviceId == deviceId && oldService.ServiceIndex == serviceIndex)
                    return; // binding did not change

                JDDevice device;
                if (!this.Device.Bus.TryGetDevice(deviceId, out device))
                    return; // device does not exist

                var service = device.GetService(serviceIndex);
                if (service == null)
                    return; // service index out of range

                // unbind previous role if needed
                Client existingRole;
                if (this.TryGetBinding(service, out existingRole) && existingRole != role)
                {
                    this.LogDebug($"unbind {existingRole}");
                    existingRole.BoundService = null;
                }

                if (oldService != null)
                    this.LogDebug($"unbind {role}");
                // bind to new role
                role.BoundService = service;
                this.LogDebug($"bind {role} to {did}[{serviceIndex}]");
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

        private bool TryGetBinding(JDService service, out Client binding)
        {
            var bindings = this.roles;
            foreach (var b in bindings)
            {
                if (b.BoundService == service)
                {
                    binding = b;
                    return true;
                }
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

        public void AutoBindRolesMaybe()
        {
            var now = this.Bus.Timestamp;
            var age = (now - this.LastBindRoles).TotalMilliseconds;
            if (age < Constants.ROLE_MANAGER_AUTO_BIND_INTERVAL && this.AutoBind)
                this.BindRoles();
        }

        public void BindRoles()
        {
            if (this.binding) return;
            lock (this.roles)
            {
                if (this.binding) return;
                try
                {
                    this.binding = true;
                    this.SyncBindRoles();
                }
                finally
                {
                    this.binding = false;
                }
            }
        }

        private void SyncBindRoles()
        {
            var bus = this.Device.Bus;
            if (bus == null) return;

            var hash = this.ComputeHash();
            this.LastBindRoles = bus.Timestamp;
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


            this.LogDebug($"bound {bound}/{roles.Length}");
            for (var i = 0; i < roles.Length; ++i)
                this.LogDebug($"  {roles[i]}");
            // try to load bindings from storage
            if (this.Storage != null)
            {
                // roles and devices are already pre-sorted
                foreach (var role in roles)
                {
                    // already bound?
                    if (role.BoundService != null) continue;

                    // try get binding from storage
                    if (this.TryLoadBinding(devices, role))
                    {
                        this.LogDebug($"storage bind {role}");
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
                                this.LogDebug($"auto bind {role}");
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

