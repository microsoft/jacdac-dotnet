namespace Jacdac {
    // Service: Azure IoT Hub Health
    public static class AzureIotHubHealthConstants
    {
        public const uint ServiceClass = 0x1462eefc;
    }

    public enum AzureIotHubHealthConnectionStatus: ushort { // uint16_t
        Connected = 0x1,
        Disconnected = 0x2,
        Connecting = 0x3,
        Disconnecting = 0x4,
    }

    public enum AzureIotHubHealthReg {
        /**
         * Read-only string (bytes). Something like `my-iot-hub.azure-devices.net` if available.
         *
         * ```
         * const [hubName] = jdunpack<[string]>(buf, "s")
         * ```
         */
        HubName = 0x180,

        /**
         * Read-only string (bytes). Device identifier in Azure Iot Hub if available.
         *
         * ```
         * const [hubDeviceId] = jdunpack<[string]>(buf, "s")
         * ```
         */
        HubDeviceId = 0x181,

        /**
         * Read-only ConnectionStatus (uint16_t). Indicates the status of connection. A message beyond the [0..3] range represents an HTTP error code.
         *
         * ```
         * const [connectionStatus] = jdunpack<[AzureIotHubHealthConnectionStatus]>(buf, "u16")
         * ```
         */
        ConnectionStatus = 0x182,
    }

    public static class AzureIotHubHealthRegPack {
        /**
         * Pack format for 'hub_name' register data.
         */
        public const string HubName = "s";

        /**
         * Pack format for 'hub_device_id' register data.
         */
        public const string HubDeviceId = "s";

        /**
         * Pack format for 'connection_status' register data.
         */
        public const string ConnectionStatus = "u16";
    }

    public enum AzureIotHubHealthCmd {
        /**
         * No args. Starts a connection to the IoT hub service
         */
        Connect = 0x81,

        /**
         * No args. Starts disconnecting from the IoT hub service
         */
        Disconnect = 0x82,

        /**
         * Argument: connection_string string (bytes). Restricted command to override the existing connection string to the Azure IoT Hub.
         *
         * ```
         * const [connectionString] = jdunpack<[string]>(buf, "s")
         * ```
         */
        SetConnectionString = 0x86,
    }

    public static class AzureIotHubHealthCmdPack {
        /**
         * Pack format for 'set_connection_string' register data.
         */
        public const string SetConnectionString = "s";
    }

    public enum AzureIotHubHealthEvent {
        /**
         * Argument: connection_status ConnectionStatus (uint16_t). Raised when the connection status changes
         *
         * ```
         * const [connectionStatus] = jdunpack<[AzureIotHubHealthConnectionStatus]>(buf, "u16")
         * ```
         */
        ConnectionStatusChange = 0x3,

        /**
         * Raised when a message has been sent to the hub.
         */
        MessageSent = 0x80,
    }

    public static class AzureIotHubHealthEventPack {
        /**
         * Pack format for 'connection_status_change' register data.
         */
        public const string ConnectionStatusChange = "u16";
    }

}
