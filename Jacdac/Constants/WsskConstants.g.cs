namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Wssk = 0x13b739fe;
    }

    public enum WsskStreamingType: ushort { // uint16_t
        Jacdac = 0x1,
        Dmesg = 0x2,
        Exceptions = 0x100,
        TemporaryMask = 0xff,
        PermamentMask = 0xff00,
        DefaultMask = 0x100,
    }


    public enum WsskDataType: byte { // uint8_t
        Binary = 0x1,
        JSON = 0x2,
    }

    public enum WsskCmd : ushort {
        /// <summary>
        /// Argument: message string (bytes). Issued when a command fails.
        ///
        /// ```
        /// const [message] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        Error = 0xff,

        /// <summary>
        /// Argument: status StreamingType (uint16_t). Enable/disable forwarding of all Jacdac frames, exception reporting, and `dmesg` streaming.
        ///
        /// ```
        /// const [status] = jdunpack<[WsskStreamingType]>(buf, "u16")
        /// ```
        /// </summary>
        SetStreaming = 0x90,

        /// <summary>
        /// Argument: payload bytes. Send from gateway when it wants to see if the device is alive.
        /// The device currently only support 0-length payload.
        ///
        /// ```
        /// const [payload] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        PingDevice = 0x91,

        /// <summary>
        /// report PingDevice
        /// ```
        /// const [payload] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>

        /// <summary>
        /// Argument: payload bytes. Send from device to gateway to test connection.
        ///
        /// ```
        /// const [payload] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        PingCloud = 0x92,

        /// <summary>
        /// report PingCloud
        /// ```
        /// const [payload] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>

        /// <summary>
        /// No args. Get SHA256 of the currently deployed program.
        /// </summary>
        GetHash = 0x93,

        /// <summary>
        /// report GetHash
        /// ```
        /// const [sha256] = jdunpack<[Uint8Array]>(buf, "b[32]")
        /// ```
        /// </summary>

        /// <summary>
        /// Argument: size B uint32_t. Start deployment of a new program.
        ///
        /// ```
        /// const [size] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        DeployStart = 0x94,

        /// <summary>
        /// Argument: payload bytes. Payload length must be multiple of 32 bytes.
        ///
        /// ```
        /// const [payload] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        DeployWrite = 0x95,

        /// <summary>
        /// No args. Finish deployment.
        /// </summary>
        DeployFinish = 0x96,

        /// <summary>
        /// Upload a labelled tuple of values to the cloud.
        /// The tuple will be automatically tagged with timestamp and originating device.
        ///
        /// ```
        /// const [datatype, topic, payload] = jdunpack<[WsskDataType, string, Uint8Array]>(buf, "u8 z b")
        /// ```
        /// </summary>
        C2d = 0x97,

        /// <summary>
        /// Upload a binary message to the cloud.
        ///
        /// ```
        /// const [datatype, topic, payload] = jdunpack<[WsskDataType, string, Uint8Array]>(buf, "u8 z b")
        /// ```
        /// </summary>
        D2c = 0x98,

        /// <summary>
        /// Argument: frame bytes. Sent both ways.
        ///
        /// ```
        /// const [frame] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        JacdacPacket = 0x99,

        /// <summary>
        /// report JacdacPacket
        /// ```
        /// const [frame] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>

        /// <summary>
        /// Argument: logs bytes. The `logs` field is string in UTF-8 encoding, however it can be split in the middle of UTF-8 code point.
        /// Controlled via `dmesg_en`.
        ///
        /// ```
        /// const [logs] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        Dmesg = 0x9a,

        /// <summary>
        /// Argument: logs bytes. The format is the same as `dmesg` but this is sent on exceptions only and is controlled separately via `exception_en`.
        ///
        /// ```
        /// const [logs] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        ExceptionReport = 0x9b,
    }

    public static class WsskCmdPack {
        /// <summary>
        /// Pack format for 'error' data.
        /// </summary>
        public const string Error = "s";

        /// <summary>
        /// Pack format for 'set_streaming' data.
        /// </summary>
        public const string SetStreaming = "u16";

        /// <summary>
        /// Pack format for 'ping_device' data.
        /// </summary>
        public const string PingDevice = "b";

        /// <summary>
        /// Pack format for 'ping_device' data.
        /// </summary>
        public const string PingDeviceReport = "b";

        /// <summary>
        /// Pack format for 'ping_cloud' data.
        /// </summary>
        public const string PingCloud = "b";

        /// <summary>
        /// Pack format for 'ping_cloud' data.
        /// </summary>
        public const string PingCloudReport = "b";

        /// <summary>
        /// Pack format for 'get_hash' data.
        /// </summary>
        public const string GetHashReport = "b[32]";

        /// <summary>
        /// Pack format for 'deploy_start' data.
        /// </summary>
        public const string DeployStart = "u32";

        /// <summary>
        /// Pack format for 'deploy_write' data.
        /// </summary>
        public const string DeployWrite = "b";

        /// <summary>
        /// Pack format for 'c2d' data.
        /// </summary>
        public const string C2d = "u8 z b";

        /// <summary>
        /// Pack format for 'd2c' data.
        /// </summary>
        public const string D2c = "u8 z b";

        /// <summary>
        /// Pack format for 'jacdac_packet' data.
        /// </summary>
        public const string JacdacPacket = "b";

        /// <summary>
        /// Pack format for 'jacdac_packet' data.
        /// </summary>
        public const string JacdacPacketReport = "b";

        /// <summary>
        /// Pack format for 'dmesg' data.
        /// </summary>
        public const string Dmesg = "b";

        /// <summary>
        /// Pack format for 'exception_report' data.
        /// </summary>
        public const string ExceptionReport = "b";
    }

}
