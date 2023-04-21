namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Logger = 0x12dc1fca;
    }

    public enum LoggerPriority: byte { // uint8_t
        Debug = 0x0,
        Log = 0x1,
        Warning = 0x2,
        Error = 0x3,
        Silent = 0x4,
    }

    public enum LoggerReg : ushort {
        /// <summary>
        /// Read-write Priority (uint8_t). Messages with level lower than this won't be emitted. The default setting may vary.
        /// Loggers should revert this to their default setting if the register has not been
        /// updated in 3000ms, and also keep the lowest setting they have seen in the last 1500ms.
        /// Thus, clients should write this register every 1000ms and ignore messages which are
        /// too verbose for them.
        ///
        /// ```
        /// const [minPriority] = jdunpack<[LoggerPriority]>(buf, "u8")
        /// ```
        /// </summary>
        MinPriority = 0x80,
    }

    public static class LoggerRegPack {
        /// <summary>
        /// Pack format for 'min_priority' data.
        /// </summary>
        public const string MinPriority = "u8";
    }

    public enum LoggerCmd : ushort {
        /// <summary>
        /// Argument: message string (bytes). Report a message.
        ///
        /// ```
        /// const [message] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        Debug = 0x80,

        /// <summary>
        /// Argument: message string (bytes). Report a message.
        ///
        /// ```
        /// const [message] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        Log = 0x81,

        /// <summary>
        /// Argument: message string (bytes). Report a message.
        ///
        /// ```
        /// const [message] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        Warn = 0x82,

        /// <summary>
        /// Argument: message string (bytes). Report a message.
        ///
        /// ```
        /// const [message] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        Error = 0x83,
    }

    public static class LoggerCmdPack {
        /// <summary>
        /// Pack format for 'debug' data.
        /// </summary>
        public const string Debug = "s";

        /// <summary>
        /// Pack format for 'log' data.
        /// </summary>
        public const string Log = "s";

        /// <summary>
        /// Pack format for 'warn' data.
        /// </summary>
        public const string Warn = "s";

        /// <summary>
        /// Pack format for 'error' data.
        /// </summary>
        public const string Error = "s";
    }

}
