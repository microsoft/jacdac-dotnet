using System;
using System.Diagnostics;

namespace Jacdac.Servers
{
    public sealed partial class RoleManagerServer : JDServiceServer
    {
        public JDStaticRegisterServer AutoBind;
        public JDStaticRegisterServer AllRolesAllocated;

        private Client[] roles = new Client[0];

        public RoleManagerServer()
            : base(Jacdac.RoleManagerConstants.ServiceClass, null)
        {
            this.AddRegister(this.AutoBind = new JDStaticRegisterServer((ushort)Jacdac.RoleManagerReg.AutoBind, Jacdac.RoleManagerRegPack.AutoBind, new object[] { 1 }));
            this.AddRegister(this.AllRolesAllocated = new JDStaticRegisterServer((ushort)Jacdac.RoleManagerReg.AllRolesAllocated, Jacdac.RoleManagerRegPack.AllRolesAllocated, new object[] { false }));
            this.AddCommand((ushort)Jacdac.RoleManagerCmd.SetRole, this.handleSetRole);
            this.AddCommand((ushort)Jacdac.RoleManagerCmd.ListRoles, this.handleListRoles);
            this.AddCommand((ushort)Jacdac.RoleManagerCmd.ClearAllRoles, this.handleClearAllRoles);

            this.Changed += this.handleChanged;
        }

        public Client[] Roles
        {
            get { return this.Roles; }
        }

        public void AddClient(Client role)
        {
            Client binding;
            if (this.TryGetClient(role.Name, out binding))
                throw new ArgumentException("role already allocated");

            var roles = this.roles;
            var newRoles = new Client[roles.Length + 1];
            roles.CopyTo(newRoles, 0);
            newRoles[roles.Length] = role;

            this.roles = newRoles;
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
            this.AllRolesAllocated.SetValues(new object[] { allRolesAllocated });
            this.SendEvent((ushort)Jacdac.RoleManagerEvent.Change);
        }

        private void handleClearAllRoles(JDNode sensor, PacketEventArgs args)
        {
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

            Client role;
            if (!this.TryGetClient(name, out role))
                return; // role does not exist

            if (role.BoundService != null && role.BoundService.Device.DeviceId == deviceId && role.BoundService.ServiceIndex == serviceIndex)
                return; // already bound

            JDDevice device;
            if (!this.Device.Bus.TryGetDevice(deviceId, out device))
                return; // device does not exist

            role.BoundService = device.GetService(serviceIndex);
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
            var hash = this.ComputeHash();
            var bus = this.Device.Bus;
            var devices = bus.GetDevices();
            var roles = this.roles;
            var bound = 0;
            // check that services are still current
            foreach (var role in roles)
            {
                if (role.BoundService != null && role.BoundService.Device.Bus == null)
                    role.BoundService = null;
                if (role.BoundService != null)
                    bound++;
            }

            // nothing to do here
            if (bound == roles.Length)
                return;

            // eager allocation strategy
            Debug.WriteLine($"roles: binding {bound}/{roles.Length}");
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
                            break;
                        }
                    }
                    // found a bound
                    if (role.BoundService != null)
                        break;
                }
            }

            if (hash != this.ComputeHash())
                this.RaiseChanged();
        }
    }
}
