namespace Jacdac {
    // Service: Role Manager
    public static class RoleManagerConstants
    {
        public const uint ServiceClass = 0x1e4b7e66;
    }
    public enum RoleManagerReg {
        /**
         * Read-write bool (uint8_t). Normally, if some roles are unfilled, and there are idle services that can fulfill them,
         * the brain device will assign roles (bind) automatically.
         * Such automatic assignment happens every second or so, and is trying to be smart about
         * co-locating roles that share "host" (part before first slash),
         * as well as reasonably stable assignments.
         * Once user start assigning roles manually using this service, auto-binding should be disabled to avoid confusion.
         *
         * ```
         * const [autoBind] = jdunpack<[number]>(buf, "u8")
         * ```
         */
        AutoBind = 0x80,

        /**
         * Read-only bool (uint8_t). Indicates if all required roles have been allocated to devices.
         *
         * ```
         * const [allRolesAllocated] = jdunpack<[number]>(buf, "u8")
         * ```
         */
        AllRolesAllocated = 0x181,
    }

    public static class RoleManagerRegPack {
        /**
         * Pack format for 'auto_bind' register data.
         */
        public const string AutoBind = "u8";

        /**
         * Pack format for 'all_roles_allocated' register data.
         */
        public const string AllRolesAllocated = "u8";
    }

    public enum RoleManagerCmd {
        /**
         * Get the role corresponding to given device identifer. Returns empty string if unset.
         *
         * ```
         * const [deviceId, serviceIdx] = jdunpack<[Uint8Array, number]>(buf, "b[8] u8")
         * ```
         */
        GetRole = 0x80,

        /**
         * report GetRole
         * ```
         * const [deviceId, serviceIdx, role] = jdunpack<[Uint8Array, number, string]>(buf, "b[8] u8 s")
         * ```
         */

        /**
         * Set role. Can set to empty to remove role binding.
         *
         * ```
         * const [deviceId, serviceIdx, role] = jdunpack<[Uint8Array, number, string]>(buf, "b[8] u8 s")
         * ```
         */
        SetRole = 0x81,

        /**
         * No args. Remove all role bindings.
         */
        ClearAllRoles = 0x84,

        /**
         * Argument: roles pipe (bytes). List all roles and bindings required by the current program. `device_id` and `service_idx` are `0` if role is unbound.
         *
         * ```
         * const [roles] = jdunpack<[Uint8Array]>(buf, "b[12]")
         * ```
         */
        ListRoles = 0x83,
    }

    public static class RoleManagerCmdPack {
        /**
         * Pack format for 'get_role' register data.
         */
        public const string GetRole = "b[8] u8";

        /**
         * Pack format for 'get_role' register data.
         */
        public const string GetRoleReport = "b[8] u8 s";

        /**
         * Pack format for 'set_role' register data.
         */
        public const string SetRole = "b[8] u8 s";

        /**
         * Pack format for 'list_roles' register data.
         */
        public const string ListRoles = "b[12]";
    }


    /**
     * pipe_report Roles
     * ```
     * const [deviceId, serviceClass, serviceIdx, role] = jdunpack<[Uint8Array, number, number, string]>(buf, "b[8] u32 u8 s")
     * ```
     */


    public static class RoleManagerinfoPack {
        /**
         * Pack format for 'roles' register data.
         */
        public const string Roles = "b[8] u32 u8 s";
    }

    public enum RoleManagerEvent {
        /**
         * Notifies that role bindings have changed.
         */
        Change = 0x3,
    }

}
