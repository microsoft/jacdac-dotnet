namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint UvIndex = 0x1f6e0d90;
    }

    public enum UvIndexVariant: byte { // uint8_t
        UVA_UVB = 0x1,
        Visible_IR = 0x2,
    }

    public enum UvIndexReg {
        /// <summary>
        /// Read-only uv u16.16 (uint32_t). Ultraviolet index, typically refreshed every second.
        ///
        /// ```
        /// const [uvIndex] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        UvIndex = 0x101,

        /// <summary>
        /// Read-only uv u16.16 (uint32_t). Error on the UV measure.
        ///
        /// ```
        /// const [uvIndexError] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        UvIndexError = 0x106,

        /// <summary>
        /// Constant Variant (uint8_t). The type of physical sensor and capabilities.
        ///
        /// ```
        /// const [variant] = jdunpack<[UvIndexVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class UvIndexRegPack {
        /// <summary>
        /// Pack format for 'uv_index' register data.
        /// </summary>
        public const string UvIndex = "u16.16";

        /// <summary>
        /// Pack format for 'uv_index_error' register data.
        /// </summary>
        public const string UvIndexError = "u16.16";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";
    }

}
