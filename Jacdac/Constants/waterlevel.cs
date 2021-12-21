namespace Jacdac {
    // Service: Water level
    public static class WaterLevelConstants
    {
        public const uint ServiceClass = 0x147b62ed;
    }

    public enum WaterLevelVariant: byte { // uint8_t
        Resistive = 0x1,
        ContactPhotoElectric = 0x2,
        NonContactPhotoElectric = 0x3,
    }

    public enum WaterLevelReg {
        /**
         * Read-only ratio u0.16 (uint16_t). The reported water level.
         *
         * ```
         * const [level] = jdunpack<[number]>(buf, "u0.16")
         * ```
         */
        Level = 0x101,

        /**
         * Read-only ratio u0.16 (uint16_t). The error rage on the current reading
         *
         * ```
         * const [levelError] = jdunpack<[number]>(buf, "u0.16")
         * ```
         */
        LevelError = 0x106,

        /**
         * Constant Variant (uint8_t). The type of physical sensor.
         *
         * ```
         * const [variant] = jdunpack<[WaterLevelVariant]>(buf, "u8")
         * ```
         */
        Variant = 0x107,
    }

    public static class WaterLevelRegPack {
        /**
         * Pack format for 'level' register data.
         */
        public const string Level = "u0.16";

        /**
         * Pack format for 'level_error' register data.
         */
        public const string LevelError = "u0.16";

        /**
         * Pack format for 'variant' register data.
         */
        public const string Variant = "u8";
    }

}
