namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Gyroscope = 0x1e1b06f2;
    }
    public enum GyroscopeReg {
        /// <summary>
        /// Indicates the current rates acting on gyroscope.
        ///
        /// ```
        /// const [x, y, z] = jdunpack<[number, number, number]>(buf, "i12.20 i12.20 i12.20")
        /// ```
        /// </summary>
        RotationRates = 0x101,

        /// <summary>
        /// Read-only °/s u12.20 (uint32_t). Error on the reading value.
        ///
        /// ```
        /// const [rotationRatesError] = jdunpack<[number]>(buf, "u12.20")
        /// ```
        /// </summary>
        RotationRatesError = 0x106,

        /// <summary>
        /// Read-write °/s u12.20 (uint32_t). Configures the range of rotation rates.
        /// The value will be "rounded up" to one of `max_rates_supported`.
        ///
        /// ```
        /// const [maxRate] = jdunpack<[number]>(buf, "u12.20")
        /// ```
        /// </summary>
        MaxRate = 0x8,

        /// <summary>
        /// Constant. Lists values supported for writing `max_rate`.
        ///
        /// ```
        /// const [maxRate] = jdunpack<[number[]]>(buf, "u12.20[]")
        /// ```
        /// </summary>
        MaxRatesSupported = 0x10a,
    }

    public static class GyroscopeRegPack {
        /// <summary>
        /// Pack format for 'rotation_rates' register data.
        /// </summary>
        public const string RotationRates = "i12.20 i12.20 i12.20";

        /// <summary>
        /// Pack format for 'rotation_rates_error' register data.
        /// </summary>
        public const string RotationRatesError = "u12.20";

        /// <summary>
        /// Pack format for 'max_rate' register data.
        /// </summary>
        public const string MaxRate = "u12.20";

        /// <summary>
        /// Pack format for 'max_rates_supported' register data.
        /// </summary>
        public const string MaxRatesSupported = "r: u12.20";
    }

}
