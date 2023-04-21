namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint AirPressure = 0x1e117cea;
    }
    public enum AirPressureReg : ushort {
        /// <summary>
        /// Read-only hPa u22.10 (uint32_t). The air pressure.
        ///
        /// ```
        /// const [pressure] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        Pressure = 0x101,

        /// <summary>
        /// Read-only hPa u22.10 (uint32_t). The real pressure is between `pressure - pressure_error` and `pressure + pressure_error`.
        ///
        /// ```
        /// const [pressureError] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        PressureError = 0x106,

        /// <summary>
        /// Constant hPa u22.10 (uint32_t). Lowest air pressure that can be reported.
        ///
        /// ```
        /// const [minPressure] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        MinPressure = 0x104,

        /// <summary>
        /// Constant hPa u22.10 (uint32_t). Highest air pressure that can be reported.
        ///
        /// ```
        /// const [maxPressure] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        MaxPressure = 0x105,
    }

    public static class AirPressureRegPack {
        /// <summary>
        /// Pack format for 'pressure' data.
        /// </summary>
        public const string Pressure = "u22.10";

        /// <summary>
        /// Pack format for 'pressure_error' data.
        /// </summary>
        public const string PressureError = "u22.10";

        /// <summary>
        /// Pack format for 'min_pressure' data.
        /// </summary>
        public const string MinPressure = "u22.10";

        /// <summary>
        /// Pack format for 'max_pressure' data.
        /// </summary>
        public const string MaxPressure = "u22.10";
    }

}
