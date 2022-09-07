namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint DcCurrentMeasurement = 0x1912c8ae;
    }
    public enum DcCurrentMeasurementReg : ushort {
        /// <summary>
        /// Constant string (bytes). A string containing the net name that is being measured e.g. `POWER_DUT` or a reference e.g. `DIFF_DEV1_DEV2`. These constants can be used to identify a measurement from client code.
        ///
        /// ```
        /// const [measurementName] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        MeasurementName = 0x182,

        /// <summary>
        /// Read-only A f64 (uint64_t). The current measurement.
        ///
        /// ```
        /// const [measurement] = jdunpack<[number]>(buf, "f64")
        /// ```
        /// </summary>
        Measurement = 0x101,

        /// <summary>
        /// Read-only A f64 (uint64_t). Absolute error on the reading value.
        ///
        /// ```
        /// const [measurementError] = jdunpack<[number]>(buf, "f64")
        /// ```
        /// </summary>
        MeasurementError = 0x106,

        /// <summary>
        /// Constant A f64 (uint64_t). Minimum measurable current
        ///
        /// ```
        /// const [minMeasurement] = jdunpack<[number]>(buf, "f64")
        /// ```
        /// </summary>
        MinMeasurement = 0x104,

        /// <summary>
        /// Constant A f64 (uint64_t). Maximum measurable current
        ///
        /// ```
        /// const [maxMeasurement] = jdunpack<[number]>(buf, "f64")
        /// ```
        /// </summary>
        MaxMeasurement = 0x105,
    }

    public static class DcCurrentMeasurementRegPack {
        /// <summary>
        /// Pack format for 'measurement_name' register data.
        /// </summary>
        public const string MeasurementName = "s";

        /// <summary>
        /// Pack format for 'measurement' register data.
        /// </summary>
        public const string Measurement = "f64";

        /// <summary>
        /// Pack format for 'measurement_error' register data.
        /// </summary>
        public const string MeasurementError = "f64";

        /// <summary>
        /// Pack format for 'min_measurement' register data.
        /// </summary>
        public const string MinMeasurement = "f64";

        /// <summary>
        /// Pack format for 'max_measurement' register data.
        /// </summary>
        public const string MaxMeasurement = "f64";
    }

}
