namespace Jacdac {
    // Service: Real time clock
    public static class RealTimeClockConstants
    {
        public const uint ServiceClass = 0x1a8b1a28;
    }

    public enum RealTimeClockVariant: byte { // uint8_t
        Computer = 0x1,
        Crystal = 0x2,
        Cuckoo = 0x3,
    }

    public enum RealTimeClockReg {
        /**
         * Current time in 24h representation.
         *
         * ```
         * const [year, month, dayOfMonth, dayOfWeek, hour, min, sec] = jdunpack<[number, number, number, number, number, number, number]>(buf, "u16 u8 u8 u8 u8 u8 u8")
         * ```
         */
        LocalTime = 0x101,

        /**
         * Read-only s u16.16 (uint32_t). Time drift since the last call to the `set_time` command.
         *
         * ```
         * const [drift] = jdunpack<[number]>(buf, "u16.16")
         * ```
         */
        Drift = 0x180,

        /**
         * Constant ppm u16.16 (uint32_t). Error on the clock, in parts per million of seconds.
         *
         * ```
         * const [precision] = jdunpack<[number]>(buf, "u16.16")
         * ```
         */
        Precision = 0x181,

        /**
         * Constant Variant (uint8_t). The type of physical clock used by the sensor.
         *
         * ```
         * const [variant] = jdunpack<[RealTimeClockVariant]>(buf, "u8")
         * ```
         */
        Variant = 0x107,
    }

    public static class RealTimeClockRegPack {
        /**
         * Pack format for 'local_time' register data.
         */
        public const string LocalTime = "u16 u8 u8 u8 u8 u8 u8";

        /**
         * Pack format for 'drift' register data.
         */
        public const string Drift = "u16.16";

        /**
         * Pack format for 'precision' register data.
         */
        public const string Precision = "u16.16";

        /**
         * Pack format for 'variant' register data.
         */
        public const string Variant = "u8";
    }

    public enum RealTimeClockCmd {
        /**
         * Sets the current time and resets the error.
         *
         * ```
         * const [year, month, dayOfMonth, dayOfWeek, hour, min, sec] = jdunpack<[number, number, number, number, number, number, number]>(buf, "u16 u8 u8 u8 u8 u8 u8")
         * ```
         */
        SetTime = 0x80,
    }

    public static class RealTimeClockCmdPack {
        /**
         * Pack format for 'set_time' register data.
         */
        public const string SetTime = "u16 u8 u8 u8 u8 u8 u8";
    }

}
