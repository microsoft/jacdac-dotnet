namespace Jacdac {
    // Service: Barometer
    public static class BarometerConstants
    {
        public const uint ServiceClass = 0x1e117cea;
    }
    public enum BarometerReg {
        /**
         * Read-only hPa u22.10 (uint32_t). The air pressure.
         *
         * ```
         * const [pressure] = jdunpack<[number]>(buf, "u22.10")
         * ```
         */
        Pressure = 0x101,

        /**
         * Read-only hPa u22.10 (uint32_t). The real pressure is between `pressure - pressure_error` and `pressure + pressure_error`.
         *
         * ```
         * const [pressureError] = jdunpack<[number]>(buf, "u22.10")
         * ```
         */
        PressureError = 0x106,
    }

    public static class BarometerRegPack {
        /**
         * Pack format for 'pressure' register data.
         */
        public const string Pressure = "u22.10";

        /**
         * Pack format for 'pressure_error' register data.
         */
        public const string PressureError = "u22.10";
    }

}
