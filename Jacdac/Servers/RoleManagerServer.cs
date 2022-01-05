using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacdac.Servers
{
    public sealed partial class RoleManagerServer : JDServiceServer
    {
        public JDStaticRegisterServer AutoBind;
        public JDStaticRegisterServer AllRolesAllocated;

        private RoleBinding[] bindings = new RoleBinding[0];

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

        private void handleChanged(JDNode sender, EventArgs e)
        {
            // recompute all roles allocated
            var allRolesAllocated = true;
            var bindings = this.bindings;
            foreach (var binding in bindings)
                if (binding.BoundToDevice == null)
                {
                    allRolesAllocated = false;
                    break;
                }
            this.AllRolesAllocated.SetValues(new object[] { allRolesAllocated });
            this.SendEvent((ushort)Jacdac.RoleManagerEvent.Change);
        }

        private void handleClearAllRoles(JDNode sensor, PacketEventArgs args)
        {
            var bindings = this.bindings;
            foreach (var binding in bindings)
                binding.Clear();
            this.RaiseChanged();
        }

        private void handleListRoles(JDNode sensor, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var pipe = OutPipe.From(this.Device.Bus, pkt);
            var bindings = this.bindings;
            pipe?.RespondForEach(bindings, k =>
            {
                var binding = (RoleBinding)k;
                var did = binding.BoundToDevice == null ? new byte[0] : HexEncoding.ToBuffer(binding.BoundToDevice);
                var role = binding.Role;
                return PacketEncoding.Pack("b[8] u32 u8 s", new object[] { did, binding.ServiceClass, binding.BoundToServiceIndex, role });
            });
        }

        private void handleSetRole(JDNode sensor, PacketEventArgs args)
        {
            var pkt = args.Packet;
            var values = PacketEncoding.UnPack(Jacdac.RoleManagerCmdPack.SetRole, pkt.Data);
            var did = (byte[])values[0];
            var deviceId = HexEncoding.ToString(did);
            var serviceIndex = (uint)values[1];
            var role = (string)values[2];

            RoleBinding binding;
            if (!this.TryGetBinding(role, out binding))
                return; // role does not exist
            if (binding.BoundToDevice == deviceId && binding.BoundToServiceIndex == serviceIndex)
                return; // already bound

            binding.Select(deviceId, serviceIndex);
            this.RaiseChanged();
        }

        private bool TryGetBinding(string role, out RoleBinding binding)
        {
            var bindings = this.bindings;
            foreach (var b in bindings)
                if (b.Role == role)
                {
                    binding = b;
                    return true;
                }
            binding = null;
            return false;
        }

        class RoleBinding
        {
            public readonly string Role;
            public readonly uint ServiceClass;
            public string BoundToDevice;
            public uint BoundToServiceIndex = 0;

            public RoleBinding(string role, uint serviceClass)
            {
                this.Role = role;
                this.ServiceClass = serviceClass;
            }

            public string Host
            {
                get
                {
                    var i = this.Role.IndexOf("/");
                    if (i == -1)
                        return this.Role;
                    else
                        return this.Role.Substring(0, i);
                }
            }

            public void Clear()
            {
                this.BoundToDevice = null;
                this.BoundToServiceIndex = 0;
            }

            public void Select(string device, uint serviceIndex)
            {
                if (device == this.BoundToDevice && serviceIndex == this.BoundToServiceIndex)
                    return;

                this.BoundToDevice = device;
                this.BoundToServiceIndex = serviceIndex;
            }
        }
    }
}
