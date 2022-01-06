namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Temperature = 0x1421bac7;
    }

    public enum TemperatureVariant: byte { // uint8_t
        Outdoor = 0x1,
        Indoor = 0x2,
        Body = 0x3,
    }

    public enum TemperatureReg {
        /// <summary>
        /// Read-only °C i22.10 (int32_t). The temperature.
        ///
        /// ```
        /// const [temperature] = jdunpack<[number]>(buf, "i22.10")
        /// ```
        /// </summary>
        Temperature = 0x101,

        /// <summary>
        /// Constant °C i22.10 (int32_t). Lowest temperature that can be reported.
        ///
        /// ```
        /// const [minTemperature] = jdunpack<[number]>(buf, "i22.10")
        /// ```
        /// </summary>
        MinTemperature = 0x104,

        /// <summary>
        /// Constant °C i22.10 (int32_t). Highest temperature that can be reported.
        ///
        /// ```
        /// const [maxTemperature] = jdunpack<[number]>(buf, "i22.10")
        /// ```
        /// </summary>
        MaxTemperature = 0x105,

        /// <summary>
        /// Read-only °C u22.10 (uint32_t). The real temperature is between `temperature - temperature_error` and `temperature + temperature_error`.
        ///
        /// ```
        /// const [temperatureError] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        TemperatureError = 0x106,

        /// <summary>
        /// Constant Variant (uint8_t). Specifies the type of thermometer.
        ///
        /// ```
        /// const [variant] = jdunpack<[TemperatureVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class TemperatureRegPack {
        /// <summary>
        /// Pack format for 'temperature' register data.
        /// </summary>
        public const string Temperature = "i22.10";

        /// <summary>
        /// Pack format for 'min_temperature' register data.
        /// </summary>
        public const string MinTemperature = "i22.10";

        /// <summary>
        /// Pack format for 'max_temperature' register data.
        /// </summary>
        public const string MaxTemperature = "i22.10";

        /// <summary>
        /// Pack format for 'temperature_error' register data.
        /// </summary>
        public const string TemperatureError = "u22.10";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";
    }

}
