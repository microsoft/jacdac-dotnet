namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint SevenSegmentDisplay = 0x196158f7;
    }
    public enum SevenSegmentDisplayReg : ushort {
        /// <summary>
        /// Read-write bytes. Each byte encodes the display status of a digit using,
        /// where lowest bit 0 encodes segment `A`, bit 1 encodes segments `B`, ..., bit 6 encodes segments `G`, and bit 7 encodes the decimal point (if present).
        /// If incoming `digits` data is smaller than `digit_count`, the remaining digits will be cleared.
        /// Thus, sending an empty `digits` payload clears the screen.
        ///
        /// ```
        /// const [digits] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        Digits = 0x2,

        /// <summary>
        /// Read-write ratio u0.16 (uint16_t). Controls the brightness of the LEDs. `0` means off.
        ///
        /// ```
        /// const [brightness] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        Brightness = 0x1,

        /// <summary>
        /// Read-write bool (uint8_t). Turn on or off the column LEDs (separating minutes from hours, etc.) in of the segment.
        /// If the column LEDs is not supported, the value remains false.
        ///
        /// ```
        /// const [doubleDots] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        DoubleDots = 0x80,

        /// <summary>
        /// Constant uint8_t. The number of digits available on the display.
        ///
        /// ```
        /// const [digitCount] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        DigitCount = 0x180,

        /// <summary>
        /// Constant bool (uint8_t). True if decimal points are available (on all digits).
        ///
        /// ```
        /// const [decimalPoint] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        DecimalPoint = 0x181,
    }

    public static class SevenSegmentDisplayRegPack {
        /// <summary>
        /// Pack format for 'digits' data.
        /// </summary>
        public const string Digits = "b";

        /// <summary>
        /// Pack format for 'brightness' data.
        /// </summary>
        public const string Brightness = "u0.16";

        /// <summary>
        /// Pack format for 'double_dots' data.
        /// </summary>
        public const string DoubleDots = "u8";

        /// <summary>
        /// Pack format for 'digit_count' data.
        /// </summary>
        public const string DigitCount = "u8";

        /// <summary>
        /// Pack format for 'decimal_point' data.
        /// </summary>
        public const string DecimalPoint = "u8";
    }

    public enum SevenSegmentDisplayCmd : ushort {
        /// <summary>
        /// Argument: value f64 (uint64_t). Shows the number on the screen using the decimal dot if available.
        /// </summary>
        SetNumber = 0x80,
    }

    public static class SevenSegmentDisplayCmdPack {
        /// <summary>
        /// Pack format for 'set_number' data.
        /// </summary>
        public const string SetNumber = "f64";
    }

}
