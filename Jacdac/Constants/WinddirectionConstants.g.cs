namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint WindDirection = 0x186be92b;
    }
    public enum WindDirectionReg : ushort {
        /// <summary>
        /// Read-only ° uint16_t. The direction of the wind.
        ///
        /// ```
        /// const [windDirection] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        WindDirection = 0x101,

        /// <summary>
        /// Read-only ° uint16_t. Error on the wind direction reading
        ///
        /// ```
        /// const [windDirectionError] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        WindDirectionError = 0x106,
    }

    public static class WindDirectionRegPack {
        /// <summary>
        /// Pack format for 'wind_direction' data.
        /// </summary>
        public const string WindDirection = "u16";

        /// <summary>
        /// Pack format for 'wind_direction_error' data.
        /// </summary>
        public const string WindDirectionError = "u16";
    }

}
