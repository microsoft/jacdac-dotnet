namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint AzureIotHubHealth = 0x1462eefc;
    }

    public enum AzureIotHubHealthConnectionStatus: ushort { // uint16_t
        Connected = 0x1,
        Disconnected = 0x2,
        Connecting = 0x3,
        Disconnecting = 0x4,
    }

    public enum AzureIotHubHealthReg : ushort {
        /// <summary>
        /// Read-only string (bytes). Something like `my-iot-hub.azure-devices.net` if available.
        ///
        /// ```
        /// const [hubName] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        HubName = 0x180,

        /// <summary>
        /// Read-only string (bytes). Device identifier in Azure Iot Hub if available.
        ///
        /// ```
        /// const [hubDeviceId] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        HubDeviceId = 0x181,

        /// <summary>
        /// Read-only ConnectionStatus (uint16_t). Indicates the status of connection. A message beyond the [0..3] range represents an HTTP error code.
        ///
        /// ```
        /// const [connectionStatus] = jdunpack<[AzureIotHubHealthConnectionStatus]>(buf, "u16")
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

    public static class AzureIotHubHealthRegPack {
        /// <summary>
        /// Pack format for 'hub_name' register data.
        /// </summary>
        public const string HubName = "s";

        /// <summary>
        /// Pack format for 'hub_device_id' register data.
        /// </summary>
        public const string HubDeviceId = "s";

        /// <summary>
        /// Pack format for 'connection_status' register data.
        /// </summary>
        public const string ConnectionStatus = "u16";

        /// <summary>
        /// Pack format for 'push_period' register data.
        /// </summary>
        public const string PushPeriod = "u32";

        /// <summary>
        /// Pack format for 'push_watchdog_period' register data.
        /// </summary>
        public const string PushWatchdogPeriod = "u32";
    }

    public enum AzureIotHubHealthCmd : ushort {
        /// <summary>
        /// No args. Starts a connection to the IoT hub service
        /// </summary>
        Connect = 0x81,

        /// <summary>
        /// No args. Starts disconnecting from the IoT hub service
        /// </summary>
        Disconnect = 0x82,

        /// <summary>
        /// Argument: connection_string string (bytes). Restricted command to override the existing connection string to the Azure IoT Hub.
        ///
        /// ```
        /// const [connectionString] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        SetConnectionString = 0x86,
    }

    public static class AzureIotHubHealthCmdPack {
        /// <summary>
        /// Pack format for 'set_connection_string' register data.
        /// </summary>
        public const string SetConnectionString = "s";
    }

    public enum AzureIotHubHealthEvent : ushort {
        /// <summary>
        /// Argument: connection_status ConnectionStatus (uint16_t). Raised when the connection status changes
        ///
        /// ```
        /// const [connectionStatus] = jdunpack<[AzureIotHubHealthConnectionStatus]>(buf, "u16")
        /// ```
        /// </summary>
        ConnectionStatusChange = 0x3,

        /// <summary>
        /// Raised when a message has been sent to the hub.
        /// </summary>
        MessageSent = 0x80,
    }

    public static class AzureIotHubHealthEventPack {
        /// <summary>
        /// Pack format for 'connection_status_change' register data.
        /// </summary>
        public const string ConnectionStatusChange = "u16";
    }

}
