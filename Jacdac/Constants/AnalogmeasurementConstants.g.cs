namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint AnalogMeasurement = 0x1633ac19;
    }

    public enum AnalogMeasurementADCMeasurementType: byte { // uint8_t
        Absolute = 0x0,
        Differential = 0x1,
    }

    public enum AnalogMeasurementReg : ushort {
        /// <summary>
        /// Constant ADCMeasurementType (uint8_t). The type of measurement that is taking place. Absolute results are measured with respect to ground, whereas differential results are measured against another signal that is not ground.
        ///
        /// ```
        /// const [measurementType] = jdunpack<[AnalogMeasurementADCMeasurementType]>(buf, "u8")
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
        /// Read-only V f64 (uint64_t). The result of the ADC measurement.
        ///
        /// ```
        /// const [measurement] = jdunpack<[number]>(buf, "f64")
        /// ```
        /// </summary>
        Measurement = 0x101,
    }

    public static class AnalogMeasurementRegPack {
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
    }

}
