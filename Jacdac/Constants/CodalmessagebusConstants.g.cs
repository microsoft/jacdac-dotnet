namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint CodalMessageBus = 0x121ff81d;
    }
    public enum CodalMessageBusCmd {
        /// <summary>
        /// Send a message on the CODAL bus. If `source` is `0`, it is treated as wildcard.
        ///
        /// ```
        /// const [source, value] = jdunpack<[number, number]>(buf, "u16 u16")
        /// ```
        /// </summary>
        Send = 0x80,
    }

    public static class CodalMessageBusCmdPack {
        /// <summary>
        /// Pack format for 'send' register data.
        /// </summary>
        public const string Send = "u16 u16";
    }

    public enum CodalMessageBusEvent {
        /// <summary>
        /// Raised by the server is triggered by the server. The filtering logic of which event to send over Jacdac is up to the server implementation.
        ///
        /// ```
        /// const [source, value] = jdunpack<[number, number]>(buf, "u16 u16")
        /// ```
        /// </summary>
        Message = 0x80,
    }

    public static class CodalMessageBusEventPack {
        /// <summary>
        /// Pack format for 'message' register data.
        /// </summary>
        public const string Message = "u16 u16";
    }

}
