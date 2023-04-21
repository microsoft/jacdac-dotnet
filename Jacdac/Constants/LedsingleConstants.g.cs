namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint LedSingle = 0x1e3048f8;
    }

    public enum LedSingleVariant: byte { // uint8_t
        ThroughHole = 0x1,
        SMD = 0x2,
        Power = 0x3,
        Bead = 0x4,
    }

    public enum LedSingleCmd : ushort {
        /// <summary>
        /// This has the same semantics as `set_status_light` in the control service.
        ///
        /// ```
        /// const [toRed, toGreen, toBlue, speed] = jdunpack<[number, number, number, number]>(buf, "u8 u8 u8 u8")
        /// ```
        /// </summary>
        Animate = 0x80,
    }

    public static class LedSingleCmdPack {
        /// <summary>
        /// Pack format for 'animate' data.
        /// </summary>
        public const string Animate = "u8 u8 u8 u8";
    }

    public enum LedSingleReg : ushort {
        /// <summary>
        /// The current color of the LED.
        ///
        /// ```
        /// const [red, green, blue] = jdunpack<[number, number, number]>(buf, "u8 u8 u8")
        /// ```
        /// </summary>
        Color = 0x180,

        /// <summary>
        /// Read-write mA uint16_t. Limit the power drawn by the light-strip (and controller).
        ///
        /// ```
        /// const [maxPower] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        MaxPower = 0x7,

        /// <summary>
        /// Constant uint16_t. If known, specifies the number of LEDs in parallel on this device.
        ///
        /// ```
        /// const [ledCount] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        LedCount = 0x183,

        /// <summary>
        /// Constant nm uint16_t. If monochrome LED, specifies the wave length of the LED.
        ///
        /// ```
        /// const [waveLength] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        WaveLength = 0x181,

        /// <summary>
        /// Constant mcd uint16_t. The luminous intensity of the LED, at full value, in micro candella.
        ///
        /// ```
        /// const [luminousIntensity] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        LuminousIntensity = 0x182,

        /// <summary>
        /// Constant Variant (uint8_t). The physical type of LED.
        ///
        /// ```
        /// const [variant] = jdunpack<[LedSingleVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class LedSingleRegPack {
        /// <summary>
        /// Pack format for 'color' data.
        /// </summary>
        public const string Color = "u8 u8 u8";

        /// <summary>
        /// Pack format for 'max_power' data.
        /// </summary>
        public const string MaxPower = "u16";

        /// <summary>
        /// Pack format for 'led_count' data.
        /// </summary>
        public const string LedCount = "u16";

        /// <summary>
        /// Pack format for 'wave_length' data.
        /// </summary>
        public const string WaveLength = "u16";

        /// <summary>
        /// Pack format for 'luminous_intensity' data.
        /// </summary>
        public const string LuminousIntensity = "u16";

        /// <summary>
        /// Pack format for 'variant' data.
        /// </summary>
        public const string Variant = "u8";
    }

}
