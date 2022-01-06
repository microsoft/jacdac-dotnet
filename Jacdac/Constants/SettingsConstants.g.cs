namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Settings = 0x1107dc4a;
    }
    public enum SettingsCmd {
        /// <summary>
        /// Argument: key string (bytes). Get the value of given setting. If no such entry exists, the value returned is empty.
        ///
        /// ```
        /// const [key] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        Get = 0x80,

        /// <summary>
        /// report Get
        /// ```
        /// const [key, value] = jdunpack<[string, Uint8Array]>(buf, "z b")
        /// ```
        /// </summary>

        /// <summary>
        /// Set the value of a given setting.
        ///
        /// ```
        /// const [key, value] = jdunpack<[string, Uint8Array]>(buf, "z b")
        /// ```
        /// </summary>
        Set = 0x81,

        /// <summary>
        /// Argument: key string (bytes). Delete a given setting.
        ///
        /// ```
        /// const [key] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        Delete = 0x84,

        /// <summary>
        /// Argument: results pipe (bytes). Return keys of all settings.
        ///
        /// ```
        /// const [results] = jdunpack<[Uint8Array]>(buf, "b[12]")
        /// ```
        /// </summary>
        ListKeys = 0x82,

        /// <summary>
        /// Argument: results pipe (bytes). Return keys and values of all settings.
        ///
        /// ```
        /// const [results] = jdunpack<[Uint8Array]>(buf, "b[12]")
        /// ```
        /// </summary>
        List = 0x83,

        /// <summary>
        /// No args. Clears all keys.
        /// </summary>
        Clear = 0x85,
    }

    public static class SettingsCmdPack {
        /// <summary>
        /// Pack format for 'get' register data.
        /// </summary>
        public const string Get = "s";

        /// <summary>
        /// Pack format for 'get' register data.
        /// </summary>
        public const string GetReport = "z b";

        /// <summary>
        /// Pack format for 'set' register data.
        /// </summary>
        public const string Set = "z b";

        /// <summary>
        /// Pack format for 'delete' register data.
        /// </summary>
        public const string Delete = "s";

        /// <summary>
        /// Pack format for 'list_keys' register data.
        /// </summary>
        public const string ListKeys = "b[12]";

        /// <summary>
        /// Pack format for 'list' register data.
        /// </summary>
        public const string List = "b[12]";
    }


    /// <summary>
    /// pipe_report ListedKey
    /// ```
    /// const [key] = jdunpack<[string]>(buf, "s")
    /// ```
    /// </summary>

    /// <summary>
    /// pipe_report ListedEntry
    /// ```
    /// const [key, value] = jdunpack<[string, Uint8Array]>(buf, "z b")
    /// ```
    /// </summary>


    public static class SettingsinfoPack {
        /// <summary>
        /// Pack format for 'listed_key' register data.
        /// </summary>
        public const string ListedKey = "s";

        /// <summary>
        /// Pack format for 'listed_entry' register data.
        /// </summary>
        public const string ListedEntry = "z b";
    }

    public enum SettingsEvent {
        /// <summary>
        /// Notifies that some setting have been modified.
        /// </summary>
        Change = 0x3,
    }

}
