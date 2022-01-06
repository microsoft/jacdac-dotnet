namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint WindSpeed = 0x1b591bbf;
    }
    public enum WindSpeedReg {
        /// <summary>
        /// Read-only m/s u16.16 (uint32_t). The velocity of the wind.
        ///
        /// ```
        /// const [windSpeed] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        WindSpeed = 0x101,

        /// <summary>
        /// Read-only m/s u16.16 (uint32_t). Error on the reading
        ///
        /// ```
        /// const [windSpeedError] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        WindSpeedError = 0x106,

        /// <summary>
        /// Constant m/s u16.16 (uint32_t). Maximum speed that can be measured by the sensor.
        ///
        /// ```
        /// const [maxWindSpeed] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        MaxWindSpeed = 0x105,
    }

    public static class WindSpeedRegPack {
        /// <summary>
        /// Pack format for 'wind_speed' register data.
        /// </summary>
        public const string WindSpeed = "u16.16";

        /// <summary>
        /// Pack format for 'wind_speed_error' register data.
        /// </summary>
        public const string WindSpeedError = "u16.16";

        /// <summary>
        /// Pack format for 'max_wind_speed' register data.
        /// </summary>
        public const string MaxWindSpeed = "u16.16";
    }

}
