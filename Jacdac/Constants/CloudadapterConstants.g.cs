namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint CloudAdapter = 0x14606e9c;
    }
    public enum CloudAdapterCmd : ushort {
        /// <summary>
        /// Upload a JSON-encoded message to the cloud.
        ///
        /// ```
        /// const [topic, json] = jdunpack<[string, string]>(buf, "z s")
        /// ```
        /// </summary>
        UploadJson = 0x80,

        /// <summary>
        /// Upload a binary message to the cloud.
        ///
        /// ```
        /// const [topic, payload] = jdunpack<[string, Uint8Array]>(buf, "z b")
        /// ```
        /// </summary>
        UploadBinary = 0x81,
    }

    public static class CloudAdapterCmdPack {
        /// <summary>
        /// Pack format for 'upload_json' data.
        /// </summary>
        public const string UploadJson = "z s";

        /// <summary>
        /// Pack format for 'upload_binary' data.
        /// </summary>
        public const string UploadBinary = "z b";
    }

    public enum CloudAdapterReg : ushort {
        /// <summary>
        /// Read-only bool (uint8_t). Indicate whether we're currently connected to the cloud server.
        /// When offline, `upload` commands are queued.
        ///
        /// ```
        /// const [connected] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Connected = 0x180,

        /// <summary>
        /// Read-only string (bytes). User-friendly name of the connection, typically includes name of the server
        /// and/or type of cloud service (`"something.cloud.net (Provider IoT)"`).
        ///
        /// ```
        /// const [connectionName] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        ConnectionName = 0x181,
    }

    public static class CloudAdapterRegPack {
        /// <summary>
        /// Pack format for 'connected' data.
        /// </summary>
        public const string Connected = "u8";

        /// <summary>
        /// Pack format for 'connection_name' data.
        /// </summary>
        public const string ConnectionName = "s";
    }

    public enum CloudAdapterEvent : ushort {
        /// <summary>
        /// Emitted when cloud send us a JSON message.
        ///
        /// ```
        /// const [topic, json] = jdunpack<[string, string]>(buf, "z s")
        /// ```
        /// </summary>
        OnJson = 0x80,

        /// <summary>
        /// Emitted when cloud send us a binary message.
        ///
        /// ```
        /// const [topic, payload] = jdunpack<[string, Uint8Array]>(buf, "z b")
        /// ```
        /// </summary>
        OnBinary = 0x81,

        /// <summary>
        /// Emitted when we connect or disconnect from the cloud.
        /// </summary>
        Change = 0x3,
    }

    public static class CloudAdapterEventPack {
        /// <summary>
        /// Pack format for 'on_json' data.
        /// </summary>
        public const string OnJson = "z s";

        /// <summary>
        /// Pack format for 'on_binary' data.
        /// </summary>
        public const string OnBinary = "z b";
    }

}
