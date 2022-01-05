using System;

namespace Jacdac.Servers
{
    public sealed partial class RoleManagerServer : JDServiceServer
    {
        public JDStaticRegisterServer AutoBind;
        public JDStaticRegisterServer AllRolesAllocated;

        private Role[] roles = new Role[0];

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

        public Role[] Roles
        {
            get { return this.Roles; }
        }

        public void AddRole(Role role)
        {
            Role binding;
            if (this.TryGetRole(role.Name, out binding))
                throw new ArgumentException("role already allocated");

            var bindings = this.roles;
            var newBindings = new Role[bindings.Length + 1];
            bindings.CopyTo(newBindings, 0);
            newBindings[bindings.Length] = role;
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
                var binding = (Role)k;
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

            Role role;
            if (!this.TryGetRole(name, out role))
                return; // role does not exist

            if (role.BoundService != null && role.BoundService.Device.DeviceId == deviceId && role.BoundService.ServiceIndex == serviceIndex)
                return; // already bound

            JDDevice device;
            if (!this.Device.Bus.TryGetDevice(deviceId, out device))
                return; // device does not exist

            role.BoundService = device.GetService(serviceIndex);
            this.RaiseChanged();
        }

        private bool TryGetRole(string role, out Role binding)
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
    }
}
