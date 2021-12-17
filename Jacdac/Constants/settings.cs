namespace Jacdac {
    // Service: Settings
    public static class SettingsConstants
    {
        public const uint ServiceClass = 0x1107dc4a;
    }
    public enum SettingsCmd {
        /**
         * Argument: key string (bytes). Get the value of given setting. If no such entry exists, the value returned is empty.
         *
         * ```
         * const [key] = jdunpack<[string]>(buf, "s")
         * ```
         */
        Get = 0x80,

        /**
         * report Get
         * ```
         * const [key, value] = jdunpack<[string, Uint8Array]>(buf, "z b")
         * ```
         */

        /**
         * Set the value of a given setting.
         *
         * ```
         * const [key, value] = jdunpack<[string, Uint8Array]>(buf, "z b")
         * ```
         */
        Set = 0x81,

        /**
         * Argument: key string (bytes). Delete a given setting.
         *
         * ```
         * const [key] = jdunpack<[string]>(buf, "s")
         * ```
         */
        Delete = 0x84,

        /**
         * Argument: results pipe (bytes). Return keys of all settings.
         *
         * ```
         * const [results] = jdunpack<[Uint8Array]>(buf, "b[12]")
         * ```
         */
        ListKeys = 0x82,

        /**
         * Argument: results pipe (bytes). Return keys and values of all settings.
         *
         * ```
         * const [results] = jdunpack<[Uint8Array]>(buf, "b[12]")
         * ```
         */
        List = 0x83,

        /**
         * No args. Clears all keys.
         */
        Clear = 0x85,
    }

    public static class SettingsCmdPack {
        /**
         * Pack format for 'get' register data.
         */
        public const string Get = "s";

        /**
         * Pack format for 'get' register data.
         */
        public const string Get = "z b";

        /**
         * Pack format for 'set' register data.
         */
        public const string Set = "z b";

        /**
         * Pack format for 'delete' register data.
         */
        public const string Delete = "s";

        /**
         * Pack format for 'list_keys' register data.
         */
        public const string ListKeys = "b[12]";

        /**
         * Pack format for 'list' register data.
         */
        public const string List = "b[12]";
    }


    /**
     * pipe_report ListedKey
     * ```
     * const [key] = jdunpack<[string]>(buf, "s")
     * ```
     */

    /**
     * pipe_report ListedEntry
     * ```
     * const [key, value] = jdunpack<[string, Uint8Array]>(buf, "z b")
     * ```
     */


    public static class SettingsinfoPack {
        /**
         * Pack format for 'listed_key' register data.
         */
        public const string ListedKey = "s";

        /**
         * Pack format for 'listed_entry' register data.
         */
        public const string ListedEntry = "z b";
    }

    public enum SettingsEvent {
        /**
         * Notifies that some setting have been modified.
         */
        Change = 0x3,
    }

}
