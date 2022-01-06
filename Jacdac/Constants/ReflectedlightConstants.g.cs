namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint ReflectedLight = 0x126c4cb2;
    }

    public enum ReflectedLightVariant: byte { // uint8_t
        InfraredDigital = 0x1,
        InfraredAnalog = 0x2,
    }

    public enum ReflectedLightReg {
        /// <summary>
        /// Read-only ratio u0.16 (uint16_t). Reports the reflected brightness. It may be a digital value or, for some sensor, analog value.
        ///
        /// ```
        /// const [brightness] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        Brightness = 0x101,

        /// <summary>
        /// Constant Variant (uint8_t). Type of physical sensor used
        ///
        /// ```
        /// const [variant] = jdunpack<[ReflectedLightVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class ReflectedLightRegPack {
        /// <summary>
        /// Pack format for 'brightness' register data.
        /// </summary>
        public const string Brightness = "u0.16";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";
    }

}
