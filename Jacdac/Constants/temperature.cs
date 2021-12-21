namespace Jacdac {
    // Service: Temperature
    public static class TemperatureConstants
    {
        public const uint ServiceClass = 0x1421bac7;
    }

    public enum TemperatureVariant: byte { // uint8_t
        Outdoor = 0x1,
        Indoor = 0x2,
        Body = 0x3,
    }

    public enum TemperatureReg {
        /**
         * Read-only °C i22.10 (int32_t). The temperature.
         *
         * ```
         * const [temperature] = jdunpack<[number]>(buf, "i22.10")
         * ```
         */
        Temperature = 0x101,

        /**
         * Constant °C i22.10 (int32_t). Lowest temperature that can be reported.
         *
         * ```
         * const [minTemperature] = jdunpack<[number]>(buf, "i22.10")
         * ```
         */
        MinTemperature = 0x104,

        /**
         * Constant °C i22.10 (int32_t). Highest temperature that can be reported.
         *
         * ```
         * const [maxTemperature] = jdunpack<[number]>(buf, "i22.10")
         * ```
         */
        MaxTemperature = 0x105,

        /**
         * Read-only °C u22.10 (uint32_t). The real temperature is between `temperature - temperature_error` and `temperature + temperature_error`.
         *
         * ```
         * const [temperatureError] = jdunpack<[number]>(buf, "u22.10")
         * ```
         */
        TemperatureError = 0x106,

        /**
         * Constant Variant (uint8_t). Specifies the type of thermometer.
         *
         * ```
         * const [variant] = jdunpack<[TemperatureVariant]>(buf, "u8")
         * ```
         */
        Variant = 0x107,
    }

    public static class TemperatureRegPack {
        /**
         * Pack format for 'temperature' register data.
         */
        public const string Temperature = "i22.10";

        /**
         * Pack format for 'min_temperature' register data.
         */
        public const string MinTemperature = "i22.10";

        /**
         * Pack format for 'max_temperature' register data.
         */
        public const string MaxTemperature = "i22.10";

        /**
         * Pack format for 'temperature_error' register data.
         */
        public const string TemperatureError = "u22.10";

        /**
         * Pack format for 'variant' register data.
         */
        public const string Variant = "u8";
    }

}
