namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Distance = 0x141a6b8a;
    }

    public enum DistanceVariant: byte { // uint8_t
        Ultrasonic = 0x1,
        Infrared = 0x2,
        LiDAR = 0x3,
        Laser = 0x4,
    }

    public enum DistanceReg {
        /// <summary>
        /// Read-only m u16.16 (uint32_t). Current distance from the object
        ///
        /// ```
        /// const [distance] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        Distance = 0x101,

        /// <summary>
        /// Read-only m u16.16 (uint32_t). Absolute error on the reading value.
        ///
        /// ```
        /// const [distanceError] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        DistanceError = 0x106,

        /// <summary>
        /// Constant m u16.16 (uint32_t). Minimum measurable distance
        ///
        /// ```
        /// const [minRange] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        MinRange = 0x104,

        /// <summary>
        /// Constant m u16.16 (uint32_t). Maximum measurable distance
        ///
        /// ```
        /// const [maxRange] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        MaxRange = 0x105,

        /// <summary>
        /// Constant Variant (uint8_t). Determines the type of sensor used.
        ///
        /// ```
        /// const [variant] = jdunpack<[DistanceVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class DistanceRegPack {
        /// <summary>
        /// Pack format for 'distance' register data.
        /// </summary>
        public const string Distance = "u16.16";

        /// <summary>
        /// Pack format for 'distance_error' register data.
        /// </summary>
        public const string DistanceError = "u16.16";

        /// <summary>
        /// Pack format for 'min_range' register data.
        /// </summary>
        public const string MinRange = "u16.16";

        /// <summary>
        /// Pack format for 'max_range' register data.
        /// </summary>
        public const string MaxRange = "u16.16";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";
    }

}
