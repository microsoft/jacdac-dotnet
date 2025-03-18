namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Acidity = 0x1e9778c5;
    }
    public enum AcidityReg : ushort {
        /// <summary>
        /// Read-only pH u4.12 (uint16_t). The acidity, pH, of water.
        ///
        /// ```
        /// const [acidity] = jdunpack<[number]>(buf, "u4.12")
        /// ```
        /// </summary>
        Acidity = 0x101,

        /// <summary>
        /// Read-only pH u4.12 (uint16_t). Error on the acidity reading.
        ///
        /// ```
        /// const [acidityError] = jdunpack<[number]>(buf, "u4.12")
        /// ```
        /// </summary>
        AcidityError = 0x106,

        /// <summary>
        /// Constant pH u4.12 (uint16_t). Lowest acidity that can be reported.
        ///
        /// ```
        /// const [minAcidity] = jdunpack<[number]>(buf, "u4.12")
        /// ```
        /// </summary>
        MinAcidity = 0x104,

        /// <summary>
        /// Constant pH u4.12 (uint16_t). Highest acidity that can be reported.
        ///
        /// ```
        /// const [maxAcidity] = jdunpack<[number]>(buf, "u4.12")
        /// ```
        /// </summary>
        MaxAcidity = 0x105,
    }

    public static class AcidityRegPack {
        /// <summary>
        /// Pack format for 'acidity' data.
        /// </summary>
        public const string Acidity = "u4.12";

        /// <summary>
        /// Pack format for 'acidity_error' data.
        /// </summary>
        public const string AcidityError = "u4.12";

        /// <summary>
        /// Pack format for 'min_acidity' data.
        /// </summary>
        public const string MinAcidity = "u4.12";

        /// <summary>
        /// Pack format for 'max_acidity' data.
        /// </summary>
        public const string MaxAcidity = "u4.12";
    }

}
