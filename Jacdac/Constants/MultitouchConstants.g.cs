namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Multitouch = 0x1d112ab5;
    }
    public enum MultitouchReg : ushort {
        /// <summary>
        /// Read-only. Capacitance of channels. The capacitance is continuously calibrated, and a value of `0` indicates
        /// no touch, wheres a value of around `100` or more indicates touch.
        /// It's best to ignore this (unless debugging), and use events.
        ///
        /// ```
        /// const [capacitance] = jdunpack<[number[]]>(buf, "i16[]")
        /// ```
        /// </summary>
        Capacity = 0x101,
    }

    public static class MultitouchRegPack {
        /// <summary>
        /// Pack format for 'capacity' register data.
        /// </summary>
        public const string Capacity = "r: i16";
    }

    public enum MultitouchEvent : ushort {
        /// <summary>
        /// Argument: channel uint8_t. Emitted when an input is touched.
        ///
        /// ```
        /// const [channel] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Touch = 0x1,

        /// <summary>
        /// Argument: channel uint8_t. Emitted when an input is no longer touched.
        ///
        /// ```
        /// const [channel] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Release = 0x2,

        /// <summary>
        /// Argument: channel uint8_t. Emitted when an input is briefly touched. TODO Not implemented.
        ///
        /// ```
        /// const [channel] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Tap = 0x80,

        /// <summary>
        /// Argument: channel uint8_t. Emitted when an input is touched for longer than 500ms. TODO Not implemented.
        ///
        /// ```
        /// const [channel] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        LongPress = 0x81,

        /// <summary>
        /// Emitted when input channels are successively touched in order of increasing channel numbers.
        ///
        /// ```
        /// const [duration, startChannel, endChannel] = jdunpack<[number, number, number]>(buf, "u16 u8 u8")
        /// ```
        /// </summary>
        SwipePos = 0x90,

        /// <summary>
        /// Emitted when input channels are successively touched in order of decreasing channel numbers.
        ///
        /// ```
        /// const [duration, startChannel, endChannel] = jdunpack<[number, number, number]>(buf, "u16 u8 u8")
        /// ```
        /// </summary>
        SwipeNeg = 0x91,
    }

    public static class MultitouchEventPack {
        /// <summary>
        /// Pack format for 'touch' register data.
        /// </summary>
        public const string Touch = "u8";

        /// <summary>
        /// Pack format for 'release' register data.
        /// </summary>
        public const string Release = "u8";

        /// <summary>
        /// Pack format for 'tap' register data.
        /// </summary>
        public const string Tap = "u8";

        /// <summary>
        /// Pack format for 'long_press' register data.
        /// </summary>
        public const string LongPress = "u8";

        /// <summary>
        /// Pack format for 'swipe_pos' register data.
        /// </summary>
        public const string SwipePos = "u16 u8 u8";

        /// <summary>
        /// Pack format for 'swipe_neg' register data.
        /// </summary>
        public const string SwipeNeg = "u16 u8 u8";
    }

}
