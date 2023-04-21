namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint RoleManager = 0x1e4b7e66;
    }
    public enum RoleManagerReg : ushort {
        /// <summary>
        /// Read-write bool (uint8_t). Normally, if some roles are unfilled, and there are idle services that can fulfill them,
        /// the brain device will assign roles (bind) automatically.
        /// Such automatic assignment happens every second or so, and is trying to be smart about
        /// co-locating roles that share "host" (part before first slash),
        /// as well as reasonably stable assignments.
        /// Once user start assigning roles manually using this service, auto-binding should be disabled to avoid confusion.
        ///
        /// ```
        /// const [autoBind] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        AutoBind = 0x80,

        /// <summary>
        /// Read-only bool (uint8_t). Indicates if all required roles have been allocated to devices.
        ///
        /// ```
        /// const [allRolesAllocated] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        AllRolesAllocated = 0x181,
    }

    public static class RoleManagerRegPack {
        /// <summary>
        /// Pack format for 'auto_bind' data.
        /// </summary>
        public const string AutoBind = "u8";

        /// <summary>
        /// Pack format for 'all_roles_allocated' data.
        /// </summary>
        public const string AllRolesAllocated = "u8";
    }

    public enum RoleManagerCmd : ushort {
        /// <summary>
        /// Set role. Can set to empty to remove role binding.
        ///
        /// ```
        /// const [deviceId, serviceIdx, role] = jdunpack<[Uint8Array, number, string]>(buf, "b[8] u8 s")
        /// ```
        /// </summary>
        SetRole = 0x81,

        /// <summary>
        /// No args. Remove all role bindings.
        /// </summary>
        ClearAllRoles = 0x84,

        /// <summary>
        /// Argument: roles pipe (bytes). List all roles and bindings required by the current program. `device_id` and `service_idx` are `0` if role is unbound.
        ///
        /// ```
        /// const [roles] = jdunpack<[Uint8Array]>(buf, "b[12]")
        /// ```
        /// </summary>
        ListRoles = 0x83,
    }

    public static class RoleManagerCmdPack {
        /// <summary>
        /// Pack format for 'set_role' data.
        /// </summary>
        public const string SetRole = "b[8] u8 s";

        /// <summary>
        /// Pack format for 'list_roles' data.
        /// </summary>
        public const string ListRoles = "b[12]";
    }

    public enum RoleManagerPipe : ushort {
        /// <summary>
        /// pipe_report Roles
        /// ```
        /// const [deviceId, serviceClass, serviceIdx, role] = jdunpack<[Uint8Array, number, number, string]>(buf, "b[8] u32 u8 s")
        /// ```
        /// </summary>
    }

    public static class RoleManagerPipePack {
        /// <summary>
        /// Pack format for 'roles' data.
        /// </summary>
        public const string Roles = "b[8] u32 u8 s";
    }

    public enum RoleManagerEvent : ushort {
        /// <summary>
        /// Notifies that role bindings have changed.
        /// </summary>
        Change = 0x3,
    }

}
