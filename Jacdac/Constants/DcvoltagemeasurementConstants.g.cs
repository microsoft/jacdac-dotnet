namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint DcVoltageMeasurement = 0x1633ac19;
    }

    public enum DcVoltageMeasurementVoltageMeasurementType: byte { // uint8_t
        Absolute = 0x0,
        Differential = 0x1,
    }

    public enum DcVoltageMeasurementReg : ushort {
        /// <summary>
        /// Constant VoltageMeasurementType (uint8_t). The type of measurement that is taking place. Absolute results are measured with respect to ground, whereas differential results are measured against another signal that is not ground.
        ///
        /// ```
        /// const [measurementType] = jdunpack<[DcVoltageMeasurementVoltageMeasurementType]>(buf, "u8")
        /// ```
        /// </summary>
        MeasurementType = 0x181,

        /// <summary>
        /// Constant string (bytes). A string containing the net name that is being measured e.g. `POWER_DUT` or a reference e.g. `DIFF_DEV1_DEV2`. These constants can be used to identify a measurement from client code.
        ///
        /// ```
        /// const [measurementName] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        MeasurementName = 0x182,

        /// <summary>
        /// Read-only V f64 (uint64_t). The voltage measurement.
        ///
        /// ```
        /// const [measurement] = jdunpack<[number]>(buf, "f64")
        /// ```
        /// </summary>
        Measurement = 0x101,

        /// <summary>
        /// Read-only V f64 (uint64_t). Absolute error on the reading value.
        ///
        /// ```
        /// const [measurementError] = jdunpack<[number]>(buf, "f64")
        /// ```
        /// </summary>
        MeasurementError = 0x106,

        /// <summary>
        /// Constant V f64 (uint64_t). Minimum measurable current
        ///
        /// ```
        /// const [minMeasurement] = jdunpack<[number]>(buf, "f64")
        /// ```
        /// </summary>
        MinMeasurement = 0x104,

        /// <summary>
        /// Constant V f64 (uint64_t). Maximum measurable current
        ///
        /// ```
        /// const [maxMeasurement] = jdunpack<[number]>(buf, "f64")
        /// ```
        /// </summary>
        MaxMeasurement = 0x105,
    }

    public static class DcVoltageMeasurementRegPack {
        /// <summary>
        /// Pack format for 'measurement_type' register data.
        /// </summary>
        public const string MeasurementType = "u8";

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
