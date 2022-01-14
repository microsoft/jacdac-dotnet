namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Barometer = 0x1e117cea;
    }
    public enum BarometerReg : ushort {
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
    }

    public static class BarometerRegPack {
        /// <summary>
        /// Pack format for 'pressure' register data.
        /// </summary>
        public const string Pressure = "u22.10";

        /// <summary>
        /// Pack format for 'pressure_error' register data.
        /// </summary>
        public const string PressureError = "u22.10";
    }

}
