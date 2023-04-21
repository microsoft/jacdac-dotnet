namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint LightLevel = 0x17dc9a1c;
    }

    public enum LightLevelVariant: byte { // uint8_t
        PhotoResistor = 0x1,
        ReverseBiasedLED = 0x2,
    }

    public enum LightLevelReg : ushort {
        /// <summary>
        /// Read-only ratio u0.16 (uint16_t). Detect light level
        ///
        /// ```
        /// const [lightLevel] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        LightLevel = 0x101,

        /// <summary>
        /// Read-only ratio u0.16 (uint16_t). Absolute estimated error of the reading value
        ///
        /// ```
        /// const [lightLevelError] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        LightLevelError = 0x106,

        /// <summary>
        /// Constant Variant (uint8_t). The type of physical sensor.
        ///
        /// ```
        /// const [variant] = jdunpack<[LightLevelVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class LightLevelRegPack {
        /// <summary>
        /// Pack format for 'light_level' data.
        /// </summary>
        public const string LightLevel = "u0.16";

        /// <summary>
        /// Pack format for 'light_level_error' data.
        /// </summary>
        public const string LightLevelError = "u0.16";

        /// <summary>
        /// Pack format for 'variant' data.
        /// </summary>
        public const string Variant = "u8";
    }

}
