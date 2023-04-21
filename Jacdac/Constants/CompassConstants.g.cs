namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Compass = 0x15b7b9bf;
    }
    public enum CompassReg : ushort {
        /// <summary>
        /// Read-only ° u16.16 (uint32_t). The heading with respect to the magnetic north.
        ///
        /// ```
        /// const [heading] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        Heading = 0x101,

        /// <summary>
        /// Read-write bool (uint8_t). Turn on or off the sensor. Turning on the sensor may start a calibration sequence.
        ///
        /// ```
        /// const [enabled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Enabled = 0x1,

        /// <summary>
        /// Read-only ° u16.16 (uint32_t). Error on the heading reading
        ///
        /// ```
        /// const [headingError] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        HeadingError = 0x106,
    }

    public static class CompassRegPack {
        /// <summary>
        /// Pack format for 'heading' data.
        /// </summary>
        public const string Heading = "u16.16";

        /// <summary>
        /// Pack format for 'enabled' data.
        /// </summary>
        public const string Enabled = "u8";

        /// <summary>
        /// Pack format for 'heading_error' data.
        /// </summary>
        public const string HeadingError = "u16.16";
    }

    public enum CompassCmd : ushort {
        /// <summary>
        /// No args. Starts a calibration sequence for the compass.
        /// </summary>
        Calibrate = 0x2,
    }

}
