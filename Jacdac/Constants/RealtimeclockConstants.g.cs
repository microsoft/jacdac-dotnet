namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint RealTimeClock = 0x1a8b1a28;
    }

    public enum RealTimeClockVariant: byte { // uint8_t
        Computer = 0x1,
        Crystal = 0x2,
        Cuckoo = 0x3,
    }

    public enum RealTimeClockReg : ushort {
        /// <summary>
        /// Current time in 24h representation.
        ///
        /// ```
        /// const [year, month, dayOfMonth, dayOfWeek, hour, min, sec] = jdunpack<[number, number, number, number, number, number, number]>(buf, "u16 u8 u8 u8 u8 u8 u8")
        /// ```
        /// </summary>
        LocalTime = 0x101,

        /// <summary>
        /// Read-only s u16.16 (uint32_t). Time drift since the last call to the `set_time` command.
        ///
        /// ```
        /// const [drift] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        Drift = 0x180,

        /// <summary>
        /// Constant ppm u16.16 (uint32_t). Error on the clock, in parts per million of seconds.
        ///
        /// ```
        /// const [precision] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        Precision = 0x181,

        /// <summary>
        /// Constant Variant (uint8_t). The type of physical clock used by the sensor.
        ///
        /// ```
        /// const [variant] = jdunpack<[RealTimeClockVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class RealTimeClockRegPack {
        /// <summary>
        /// Pack format for 'local_time' register data.
        /// </summary>
        public const string LocalTime = "u16 u8 u8 u8 u8 u8 u8";

        /// <summary>
        /// Pack format for 'drift' register data.
        /// </summary>
        public const string Drift = "u16.16";

        /// <summary>
        /// Pack format for 'precision' register data.
        /// </summary>
        public const string Precision = "u16.16";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";
    }

    public enum RealTimeClockCmd : ushort {
        /// <summary>
        /// Sets the current time and resets the error.
        ///
        /// ```
        /// const [year, month, dayOfMonth, dayOfWeek, hour, min, sec] = jdunpack<[number, number, number, number, number, number, number]>(buf, "u16 u8 u8 u8 u8 u8 u8")
        /// ```
        /// </summary>
        SetTime = 0x80,
    }

    public static class RealTimeClockCmdPack {
        /// <summary>
        /// Pack format for 'set_time' register data.
        /// </summary>
        public const string SetTime = "u16 u8 u8 u8 u8 u8 u8";
    }

}
