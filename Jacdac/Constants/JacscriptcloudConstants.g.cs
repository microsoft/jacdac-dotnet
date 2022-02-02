namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint JacscriptCloud = 0x14606e9c;
    }

    public enum JacscriptCloudCommandStatus: uint { // uint32_t
        OK = 0xc8,
        NotFound = 0x194,
        Busy = 0x1ad,
    }

    public enum JacscriptCloudCmd : ushort {
        /// <summary>
        /// Upload a labelled tuple of values to the cloud.
        /// The tuple will be automatically tagged with timestamp and originating device.
        ///
        /// ```
        /// const [label, value] = jdunpack<[string, number[]]>(buf, "z f64[]")
        /// ```
        /// </summary>
        Upload = 0x80,

        /// <summary>
        /// Argument: path string (bytes). Get a numeric field from the current device twin.
        /// Path is dot-separated.
        ///
        /// ```
        /// const [path] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        GetTwin = 0x81,

        /// <summary>
        /// report GetTwin
        /// ```
        /// const [path, value] = jdunpack<[string, number]>(buf, "z f64")
        /// ```
        /// </summary>

        /// <summary>
        /// Should be called by jacscript when it finishes handling a `cloud_command`.
        ///
        /// ```
        /// const [seqNo, status, result] = jdunpack<[number, JacscriptCloudCommandStatus, number[]]>(buf, "u32 u32 f64[]")
        /// ```
        /// </summary>
        AckCloudCommand = 0x83,
    }

    public static class JacscriptCloudCmdPack {
        /// <summary>
        /// Pack format for 'upload' register data.
        /// </summary>
        public const string Upload = "z r: f64";

        /// <summary>
        /// Pack format for 'get_twin' register data.
        /// </summary>
        public const string GetTwin = "s";

        /// <summary>
        /// Pack format for 'get_twin' register data.
        /// </summary>
        public const string GetTwinReport = "z f64";

        /// <summary>
        /// Pack format for 'ack_cloud_command' register data.
        /// </summary>
        public const string AckCloudCommand = "u32 u32 r: f64";
    }

    public enum JacscriptCloudReg : ushort {
        /// <summary>
        /// Read-only bool (uint8_t). Indicate whether we're currently connected to the cloud server.
        /// When offline, `upload` commands are queued, and `get_twin` respond with cached values.
        ///
        /// ```
        /// const [connected] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Connected = 0x180,
    }

    public static class JacscriptCloudRegPack {
        /// <summary>
        /// Pack format for 'connected' register data.
        /// </summary>
        public const string Connected = "u8";
    }

    public enum JacscriptCloudEvent : ushort {
        /// <summary>
        /// Emitted when cloud requests jacscript to run some action.
        ///
        /// ```
        /// const [seqNo, command, argument] = jdunpack<[number, string, number[]]>(buf, "u32 z f64[]")
        /// ```
        /// </summary>
        CloudCommand = 0x81,

        /// <summary>
        /// Emitted whenever any of the twin properties change.
        /// </summary>
        TwinChange = 0x3,
    }

    public static class JacscriptCloudEventPack {
        /// <summary>
        /// Pack format for 'cloud_command' register data.
        /// </summary>
        public const string CloudCommand = "u32 z r: f64";
    }

}
