namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint DCCurrentMeasurement = 0x1912c8ae;
    }
    public enum DCCurrentMeasurementReg : ushort {
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
    }

    public static class DCCurrentMeasurementRegPack {
        /// <summary>
        /// Pack format for 'measurement_name' register data.
        /// </summary>
        public const string MeasurementName = "s";

        /// <summary>
        /// Pack format for 'measurement' register data.
        /// </summary>
        public const string Measurement = "f64";
    }

}
