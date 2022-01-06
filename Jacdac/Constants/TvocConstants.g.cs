namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Tvoc = 0x12a5b597;
    }
    public enum TvocReg {
        /// <summary>
        /// Read-only ppb u22.10 (uint32_t). Total volatile organic compound readings in parts per billion.
        ///
        /// ```
        /// const [tVOC] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        TVOC = 0x101,

        /// <summary>
        /// Read-only ppb u22.10 (uint32_t). Error on the reading data
        ///
        /// ```
        /// const [tVOCError] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        TVOCError = 0x106,

        /// <summary>
        /// Constant ppb u22.10 (uint32_t). Minimum measurable value
        ///
        /// ```
        /// const [minTVOC] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        MinTVOC = 0x104,

        /// <summary>
        /// Constant ppb u22.10 (uint32_t). Minimum measurable value.
        ///
        /// ```
        /// const [maxTVOC] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        MaxTVOC = 0x105,
    }

    public static class TvocRegPack {
        /// <summary>
        /// Pack format for 'TVOC' register data.
        /// </summary>
        public const string TVOC = "u22.10";

        /// <summary>
        /// Pack format for 'TVOC_error' register data.
        /// </summary>
        public const string TVOCError = "u22.10";

        /// <summary>
        /// Pack format for 'min_TVOC' register data.
        /// </summary>
        public const string MinTVOC = "u22.10";

        /// <summary>
        /// Pack format for 'max_TVOC' register data.
        /// </summary>
        public const string MaxTVOC = "u22.10";
    }

}
