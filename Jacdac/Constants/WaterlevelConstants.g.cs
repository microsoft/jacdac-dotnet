namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint WaterLevel = 0x147b62ed;
    }

    public enum WaterLevelVariant: byte { // uint8_t
        Resistive = 0x1,
        ContactPhotoElectric = 0x2,
        NonContactPhotoElectric = 0x3,
    }

    public enum WaterLevelReg : ushort {
        /// <summary>
        /// Read-only ratio u0.16 (uint16_t). The reported water level.
        ///
        /// ```
        /// const [level] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        Level = 0x101,

        /// <summary>
        /// Read-only ratio u0.16 (uint16_t). The error rage on the current reading
        ///
        /// ```
        /// const [levelError] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        LevelError = 0x106,

        /// <summary>
        /// Constant Variant (uint8_t). The type of physical sensor.
        ///
        /// ```
        /// const [variant] = jdunpack<[WaterLevelVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class WaterLevelRegPack {
        /// <summary>
        /// Pack format for 'level' data.
        /// </summary>
        public const string Level = "u0.16";

        /// <summary>
        /// Pack format for 'level_error' data.
        /// </summary>
        public const string LevelError = "u0.16";

        /// <summary>
        /// Pack format for 'variant' data.
        /// </summary>
        public const string Variant = "u8";
    }

}
