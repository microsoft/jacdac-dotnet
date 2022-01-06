namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Relay = 0x183fe656;
    }

    public enum RelayVariant: byte { // uint8_t
        Electromechanical = 0x1,
        SolidState = 0x2,
        Reed = 0x3,
    }

    public enum RelayReg {
        /// <summary>
        /// Read-write bool (uint8_t). Indicates whether the relay circuit is currently energized (closed) or not.
        ///
        /// ```
        /// const [closed] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Closed = 0x1,

        /// <summary>
        /// Constant Variant (uint8_t). Describes the type of relay used.
        ///
        /// ```
        /// const [variant] = jdunpack<[RelayVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,

        /// <summary>
        /// Constant mA uint32_t. Maximum switching current for a resistive load.
        ///
        /// ```
        /// const [maxSwitchingCurrent] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        MaxSwitchingCurrent = 0x180,
    }

    public static class RelayRegPack {
        /// <summary>
        /// Pack format for 'closed' register data.
        /// </summary>
        public const string Closed = "u8";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";

        /// <summary>
        /// Pack format for 'max_switching_current' register data.
        /// </summary>
        public const string MaxSwitchingCurrent = "u32";
    }

}
