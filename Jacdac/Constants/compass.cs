namespace Jacdac {
    // Service: Compass
    public static class CompassConstants
    {
        public const uint ServiceClass = 0x15b7b9bf;
    }
    public enum CompassReg {
        /**
         * Read-only ° u16.16 (uint32_t). The heading with respect to the magnetic north.
         *
         * ```
         * const [heading] = jdunpack<[number]>(buf, "u16.16")
         * ```
         */
        Heading = 0x101,

        /**
         * Read-write bool (uint8_t). Turn on or off the sensor. Turning on the sensor may start a calibration sequence.
         *
         * ```
         * const [enabled] = jdunpack<[number]>(buf, "u8")
         * ```
         */
        Enabled = 0x1,

        /**
         * Read-only ° u16.16 (uint32_t). Error on the heading reading
         *
         * ```
         * const [headingError] = jdunpack<[number]>(buf, "u16.16")
         * ```
         */
        HeadingError = 0x106,
    }

    public static class CompassRegPack {
        /**
         * Pack format for 'heading' register data.
         */
        public const string Heading = "u16.16";

        /**
         * Pack format for 'enabled' register data.
         */
        public const string Enabled = "u8";

        /**
         * Pack format for 'heading_error' register data.
         */
        public const string HeadingError = "u16.16";
    }

    public enum CompassCmd {
        /**
         * No args. Starts a calibration sequence for the compass.
         */
        Calibrate = 0x2,
    }

}
