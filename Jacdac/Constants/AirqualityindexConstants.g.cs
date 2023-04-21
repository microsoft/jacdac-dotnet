namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint AirQualityIndex = 0x14ac6ed6;
    }
    public enum AirQualityIndexReg : ushort {
        /// <summary>
        /// Read-only AQI u16.16 (uint32_t). Air quality index, typically refreshed every second.
        ///
        /// ```
        /// const [aqiIndex] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        AqiIndex = 0x101,

        /// <summary>
        /// Read-only AQI u16.16 (uint32_t). Error on the AQI measure.
        ///
        /// ```
        /// const [aqiIndexError] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        AqiIndexError = 0x106,

        /// <summary>
        /// Constant AQI u16.16 (uint32_t). Minimum AQI reading, representing a good air quality. Typically 0.
        ///
        /// ```
        /// const [minAqiIndex] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        MinAqiIndex = 0x104,

        /// <summary>
        /// Constant AQI u16.16 (uint32_t). Maximum AQI reading, representing a very poor air quality.
        ///
        /// ```
        /// const [maxAqiIndex] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        MaxAqiIndex = 0x105,
    }

    public static class AirQualityIndexRegPack {
        /// <summary>
        /// Pack format for 'aqi_index' data.
        /// </summary>
        public const string AqiIndex = "u16.16";

        /// <summary>
        /// Pack format for 'aqi_index_error' data.
        /// </summary>
        public const string AqiIndexError = "u16.16";

        /// <summary>
        /// Pack format for 'min_aqi_index' data.
        /// </summary>
        public const string MinAqiIndex = "u16.16";

        /// <summary>
        /// Pack format for 'max_aqi_index' data.
        /// </summary>
        public const string MaxAqiIndex = "u16.16";
    }

}
