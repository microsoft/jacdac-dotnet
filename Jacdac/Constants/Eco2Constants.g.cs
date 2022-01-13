namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint ECO2 = 0x169c9dc6;
    }

    public enum ECO2Variant: byte { // uint8_t
        VOC = 0x1,
        NDIR = 0x2,
    }

    public enum ECO2Reg : ushort {
        /// <summary>
        /// Read-only ppm u22.10 (uint32_t). Equivalent CO₂ (eCO₂) readings.
        ///
        /// ```
        /// const [eCO2] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        ECO2 = 0x101,

        /// <summary>
        /// Read-only ppm u22.10 (uint32_t). Error on the reading value.
        ///
        /// ```
        /// const [eCO2Error] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        ECO2Error = 0x106,

        /// <summary>
        /// Constant ppm u22.10 (uint32_t). Minimum measurable value
        ///
        /// ```
        /// const [minECO2] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        MinECO2 = 0x104,

        /// <summary>
        /// Constant ppm u22.10 (uint32_t). Minimum measurable value
        ///
        /// ```
        /// const [maxECO2] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        MaxECO2 = 0x105,

        /// <summary>
        /// Constant Variant (uint8_t). Type of physical sensor and capabilities.
        ///
        /// ```
        /// const [variant] = jdunpack<[ECO2Variant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class ECO2RegPack {
        /// <summary>
        /// Pack format for 'e_CO2' register data.
        /// </summary>
        public const string ECO2 = "u22.10";

        /// <summary>
        /// Pack format for 'e_CO2_error' register data.
        /// </summary>
        public const string ECO2Error = "u22.10";

        /// <summary>
        /// Pack format for 'min_e_CO2' register data.
        /// </summary>
        public const string MinECO2 = "u22.10";

        /// <summary>
        /// Pack format for 'max_e_CO2' register data.
        /// </summary>
        public const string MaxECO2 = "u22.10";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";
    }

}
