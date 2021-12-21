namespace Jacdac {
    // Service: Wind direction
    public static class WindDirectionConstants
    {
        public const uint ServiceClass = 0x186be92b;
    }
    public enum WindDirectionReg {
        /**
         * Read-only ° uint16_t. The direction of the wind.
         *
         * ```
         * const [windDirection] = jdunpack<[number]>(buf, "u16")
         * ```
         */
        WindDirection = 0x101,

        /**
         * Read-only ° uint16_t. Error on the wind direction reading
         *
         * ```
         * const [windDirectionError] = jdunpack<[number]>(buf, "u16")
         * ```
         */
        WindDirectionError = 0x106,
    }

    public static class WindDirectionRegPack {
        /**
         * Pack format for 'wind_direction' register data.
         */
        public const string WindDirection = "u16";

        /**
         * Pack format for 'wind_direction_error' register data.
         */
        public const string WindDirectionError = "u16";
    }

}
