namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Humidity = 0x16c810b8;
    }
    public enum HumidityReg {
        /// <summary>
        /// Read-only %RH u22.10 (uint32_t). The relative humidity in percentage of full water saturation.
        ///
        /// ```
        /// const [humidity] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        Humidity = 0x101,

        /// <summary>
        /// Read-only %RH u22.10 (uint32_t). The real humidity is between `humidity - humidity_error` and `humidity + humidity_error`.
        ///
        /// ```
        /// const [humidityError] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        HumidityError = 0x106,

        /// <summary>
        /// Constant %RH u22.10 (uint32_t). Lowest humidity that can be reported.
        ///
        /// ```
        /// const [minHumidity] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        MinHumidity = 0x104,

        /// <summary>
        /// Constant %RH u22.10 (uint32_t). Highest humidity that can be reported.
        ///
        /// ```
        /// const [maxHumidity] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        MaxHumidity = 0x105,
    }

    public static class HumidityRegPack {
        /// <summary>
        /// Pack format for 'humidity' register data.
        /// </summary>
        public const string Humidity = "u22.10";

        /// <summary>
        /// Pack format for 'humidity_error' register data.
        /// </summary>
        public const string HumidityError = "u22.10";

        /// <summary>
        /// Pack format for 'min_humidity' register data.
        /// </summary>
        public const string MinHumidity = "u22.10";

        /// <summary>
        /// Pack format for 'max_humidity' register data.
        /// </summary>
        public const string MaxHumidity = "u22.10";
    }

}
