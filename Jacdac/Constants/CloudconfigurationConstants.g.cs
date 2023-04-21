namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint CloudConfiguration = 0x1462eefc;
    }

    public enum CloudConfigurationConnectionStatus: ushort { // uint16_t
        Connected = 0x1,
        Disconnected = 0x2,
        Connecting = 0x3,
        Disconnecting = 0x4,
    }

    public enum CloudConfigurationReg : ushort {
        /// <summary>
        /// Read-only string (bytes). Something like `my-iot-hub.azure-devices.net` if available.
        ///
        /// ```
        /// const [serverName] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        ServerName = 0x180,

        /// <summary>
        /// Read-only string (bytes). Device identifier for the device in the cloud if available.
        ///
        /// ```
        /// const [cloudDeviceId] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        CloudDeviceId = 0x181,

        /// <summary>
        /// Constant string (bytes). Cloud provider identifier.
        ///
        /// ```
        /// const [cloudType] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        CloudType = 0x183,

        /// <summary>
        /// Read-only ConnectionStatus (uint16_t). Indicates the status of connection. A message beyond the [0..3] range represents an HTTP error code.
        ///
        /// ```
        /// const [connectionStatus] = jdunpack<[CloudConfigurationConnectionStatus]>(buf, "u16")
        /// ```
        /// </summary>
        ConnectionStatus = 0x182,

        /// <summary>
        /// Read-write ms uint32_t. How often to push data to the cloud.
        ///
        /// ```
        /// const [pushPeriod] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        PushPeriod = 0x80,

        /// <summary>
        /// Read-write ms uint32_t. If no message is published within given period, the device resets.
        /// This can be due to connectivity problems or due to the device having nothing to publish.
        /// Forced to be at least `2 * flush_period`.
        /// Set to `0` to disable (default).
        ///
        /// ```
        /// const [pushWatchdogPeriod] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        PushWatchdogPeriod = 0x81,
    }

    public static class CloudConfigurationRegPack {
        /// <summary>
        /// Pack format for 'server_name' data.
        /// </summary>
        public const string ServerName = "s";

        /// <summary>
        /// Pack format for 'cloud_device_id' data.
        /// </summary>
        public const string CloudDeviceId = "s";

        /// <summary>
        /// Pack format for 'cloud_type' data.
        /// </summary>
        public const string CloudType = "s";

        /// <summary>
        /// Pack format for 'connection_status' data.
        /// </summary>
        public const string ConnectionStatus = "u16";

        /// <summary>
        /// Pack format for 'push_period' data.
        /// </summary>
        public const string PushPeriod = "u32";

        /// <summary>
        /// Pack format for 'push_watchdog_period' data.
        /// </summary>
        public const string PushWatchdogPeriod = "u32";
    }

    public enum CloudConfigurationCmd : ushort {
        /// <summary>
        /// No args. Starts a connection to the cloud service
        /// </summary>
        Connect = 0x81,

        /// <summary>
        /// No args. Starts disconnecting from the cloud service
        /// </summary>
        Disconnect = 0x82,

        /// <summary>
        /// Argument: connection_string string (bytes). Restricted command to override the existing connection string to cloud.
        ///
        /// ```
        /// const [connectionString] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        SetConnectionString = 0x86,
    }

    public static class CloudConfigurationCmdPack {
        /// <summary>
        /// Pack format for 'set_connection_string' data.
        /// </summary>
        public const string SetConnectionString = "s";
    }

    public enum CloudConfigurationEvent : ushort {
        /// <summary>
        /// Argument: connection_status ConnectionStatus (uint16_t). Raised when the connection status changes
        ///
        /// ```
        /// const [connectionStatus] = jdunpack<[CloudConfigurationConnectionStatus]>(buf, "u16")
        /// ```
        /// </summary>
        ConnectionStatusChange = 0x3,

        /// <summary>
        /// Raised when a message has been sent to the hub.
        /// </summary>
        MessageSent = 0x80,
    }

    public static class CloudConfigurationEventPack {
        /// <summary>
        /// Pack format for 'connection_status_change' data.
        /// </summary>
        public const string ConnectionStatusChange = "u16";
    }

}
