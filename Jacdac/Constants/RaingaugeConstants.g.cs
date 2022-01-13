namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint RainGauge = 0x13734c95;
    }
    public enum RainGaugeReg : ushort {
        /// <summary>
        /// Read-only mm u16.16 (uint32_t). Total precipitation recorded so far.
        ///
        /// ```
        /// const [precipitation] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        Precipitation = 0x101,

        /// <summary>
        /// Constant mm u16.16 (uint32_t). Typically the amount of rain needed for tipping the bucket.
        ///
        /// ```
        /// const [precipitationPrecision] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        PrecipitationPrecision = 0x108,
    }

    public static class RainGaugeRegPack {
        /// <summary>
        /// Pack format for 'precipitation' register data.
        /// </summary>
        public const string Precipitation = "u16.16";

        /// <summary>
        /// Pack format for 'precipitation_precision' register data.
        /// </summary>
        public const string PrecipitationPrecision = "u16.16";
    }

}
