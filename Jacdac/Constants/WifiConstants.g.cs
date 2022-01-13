namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Wifi = 0x18aae1fa;
    }

    [System.Flags]
    public enum WifiAPFlags: uint { // uint32_t
        HasPassword = 0x1,
        WPS = 0x2,
        HasSecondaryChannelAbove = 0x4,
        HasSecondaryChannelBelow = 0x8,
        IEEE_802_11B = 0x100,
        IEEE_802_11A = 0x200,
        IEEE_802_11G = 0x400,
        IEEE_802_11N = 0x800,
        IEEE_802_11AC = 0x1000,
        IEEE_802_11AX = 0x2000,
        IEEE_802_LongRange = 0x8000,
    }

    public enum WifiCmd : ushort {
        /// <summary>
        /// Argument: results pipe (bytes). Return list of WiFi network from the last scan.
        /// Scans are performed periodically while not connected (in particular, on startup and after current connection drops),
        /// as well as upon `reconnect` and `scan` commands.
        ///
        /// ```
        /// const [results] = jdunpack<[Uint8Array]>(buf, "b[12]")
        /// ```
        /// </summary>
        LastScanResults = 0x80,

        /// <summary>
        /// Automatically connect to named network if available. Also set password if network is not open.
        ///
        /// ```
        /// const [ssid, password] = jdunpack<[string, string]>(buf, "z z")
        /// ```
        /// </summary>
        AddNetwork = 0x81,

        /// <summary>
        /// No args. Enable the WiFi (if disabled), initiate a scan, wait for results, disconnect from current WiFi network if any,
        /// and then reconnect (using regular algorithm, see `set_network_priority`).
        /// </summary>
        Reconnect = 0x82,

        /// <summary>
        /// Argument: ssid string (bytes). Prevent from automatically connecting to named network in future.
        /// Forgetting a network resets its priority to `0`.
        ///
        /// ```
        /// const [ssid] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        ForgetNetwork = 0x83,

        /// <summary>
        /// No args. Clear the list of known networks.
        /// </summary>
        ForgetAllNetworks = 0x84,

        /// <summary>
        /// Set connection priority for a network.
        /// By default, all known networks have priority of `0`.
        ///
        /// ```
        /// const [priority, ssid] = jdunpack<[number, string]>(buf, "i16 s")
        /// ```
        /// </summary>
        SetNetworkPriority = 0x85,

        /// <summary>
        /// No args. Initiate search for WiFi networks. Generates `scan_complete` event.
        /// </summary>
        Scan = 0x86,

        /// <summary>
        /// Argument: results pipe (bytes). Return list of known WiFi networks.
        /// `flags` is currently always 0.
        ///
        /// ```
        /// const [results] = jdunpack<[Uint8Array]>(buf, "b[12]")
        /// ```
        /// </summary>
        ListKnownNetworks = 0x87,
    }

    public static class WifiCmdPack {
        /// <summary>
        /// Pack format for 'last_scan_results' register data.
        /// </summary>
        public const string LastScanResults = "b[12]";

        /// <summary>
        /// Pack format for 'add_network' register data.
        /// </summary>
        public const string AddNetwork = "z z";

        /// <summary>
        /// Pack format for 'forget_network' register data.
        /// </summary>
        public const string ForgetNetwork = "s";

        /// <summary>
        /// Pack format for 'set_network_priority' register data.
        /// </summary>
        public const string SetNetworkPriority = "i16 s";

        /// <summary>
        /// Pack format for 'list_known_networks' register data.
        /// </summary>
        public const string ListKnownNetworks = "b[12]";
    }


    /// <summary>
    /// pipe_report Results
    /// ```
    /// const [flags, rssi, channel, bssid, ssid] = jdunpack<[WifiAPFlags, number, number, Uint8Array, string]>(buf, "u32 x[4] i8 u8 b[6] s[33]")
    /// ```
    /// </summary>

    /// <summary>
    /// pipe_report NetworkResults
    /// ```
    /// const [priority, flags, ssid] = jdunpack<[number, number, string]>(buf, "i16 i16 s")
    /// ```
    /// </summary>


    public static class WifiinfoPack {
        /// <summary>
        /// Pack format for 'results' register data.
        /// </summary>
        public const string Results = "u32 u32 i8 u8 b[6] s[33]";

        /// <summary>
        /// Pack format for 'network_results' register data.
        /// </summary>
        public const string NetworkResults = "i16 i16 s";
    }

    public enum WifiReg : ushort {
        /// <summary>
        /// Read-write bool (uint8_t). Determines whether the WiFi radio is enabled. It starts enabled upon reset.
        ///
        /// ```
        /// const [enabled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Enabled = 0x1,

        /// <summary>
        /// Read-only bytes. 0, 4 or 16 byte buffer with the IPv4 or IPv6 address assigned to device if any.
        ///
        /// ```
        /// const [ipAddress] = jdunpack<[Uint8Array]>(buf, "b[16]")
        /// ```
        /// </summary>
        IpAddress = 0x181,

        /// <summary>
        /// Constant bytes. The 6-byte MAC address of the device. If a device does MAC address randomization it will have to "restart".
        ///
        /// ```
        /// const [eui48] = jdunpack<[Uint8Array]>(buf, "b[6]")
        /// ```
        /// </summary>
        Eui48 = 0x182,

        /// <summary>
        /// Read-only string (bytes). SSID of the access-point to which device is currently connected.
        /// Empty string if not connected.
        ///
        /// ```
        /// const [ssid] = jdunpack<[string]>(buf, "s[32]")
        /// ```
        /// </summary>
        Ssid = 0x183,

        /// <summary>
        /// Read-only dB int8_t. Current signal strength. Returns -128 when not connected.
        ///
        /// ```
        /// const [rssi] = jdunpack<[number]>(buf, "i8")
        /// ```
        /// </summary>
        Rssi = 0x184,
    }

    public static class WifiRegPack {
        /// <summary>
        /// Pack format for 'enabled' register data.
        /// </summary>
        public const string Enabled = "u8";

        /// <summary>
        /// Pack format for 'ip_address' register data.
        /// </summary>
        public const string IpAddress = "b[16]";

        /// <summary>
        /// Pack format for 'eui_48' register data.
        /// </summary>
        public const string Eui48 = "b[6]";

        /// <summary>
        /// Pack format for 'ssid' register data.
        /// </summary>
        public const string Ssid = "s[32]";

        /// <summary>
        /// Pack format for 'rssi' register data.
        /// </summary>
        public const string Rssi = "i8";
    }

    public enum WifiEvent : ushort {
        /// <summary>
        /// Emitted upon successful join and IP address assignment.
        /// </summary>
        GotIp = 0x1,

        /// <summary>
        /// Emitted when disconnected from network.
        /// </summary>
        LostIp = 0x2,

        /// <summary>
        /// A WiFi network scan has completed. Results can be read with the `last_scan_results` command.
        /// The event indicates how many networks where found, and how many are considered
        /// as candidates for connection.
        ///
        /// ```
        /// const [numNetworks, numKnownNetworks] = jdunpack<[number, number]>(buf, "u16 u16")
        /// ```
        /// </summary>
        ScanComplete = 0x80,

        /// <summary>
        /// Emitted whenever the list of known networks is updated.
        /// </summary>
        NetworksChanged = 0x81,

        /// <summary>
        /// Argument: ssid string (bytes). Emitted when when a network was detected in scan, the device tried to connect to it
        /// and failed.
        /// This may be because of wrong password or other random failure.
        ///
        /// ```
        /// const [ssid] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        ConnectionFailed = 0x82,
    }

    public static class WifiEventPack {
        /// <summary>
        /// Pack format for 'scan_complete' register data.
        /// </summary>
        public const string ScanComplete = "u16 u16";

        /// <summary>
        /// Pack format for 'connection_failed' register data.
        /// </summary>
        public const string ConnectionFailed = "s";
    }

}
