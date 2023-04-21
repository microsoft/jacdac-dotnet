namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint SoilMoisture = 0x1d4aa3b3;
    }

    public enum SoilMoistureVariant: byte { // uint8_t
        Resistive = 0x1,
        Capacitive = 0x2,
    }

    public enum SoilMoistureReg : ushort {
        /// <summary>
        /// Read-only ratio u0.16 (uint16_t). Indicates the wetness of the soil, from `dry` to `wet`.
        ///
        /// ```
        /// const [moisture] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        Moisture = 0x101,

        /// <summary>
        /// Read-only ratio u0.16 (uint16_t). The error on the moisture reading.
        ///
        /// ```
        /// const [moistureError] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        MoistureError = 0x106,

        /// <summary>
        /// Constant Variant (uint8_t). Describe the type of physical sensor.
        ///
        /// ```
        /// const [variant] = jdunpack<[SoilMoistureVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class SoilMoistureRegPack {
        /// <summary>
        /// Pack format for 'moisture' data.
        /// </summary>
        public const string Moisture = "u0.16";

        /// <summary>
        /// Pack format for 'moisture_error' data.
        /// </summary>
        public const string MoistureError = "u0.16";

        /// <summary>
        /// Pack format for 'variant' data.
        /// </summary>
        public const string Variant = "u8";
    }

}
