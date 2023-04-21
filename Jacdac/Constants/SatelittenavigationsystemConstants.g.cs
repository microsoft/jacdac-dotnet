namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint SatNav = 0x19dd6136;
    }
    public enum SatNavReg : ushort {
        /// <summary>
        /// Reported coordinates, geometric altitude and time of position. Altitude accuracy is 0 if not available.
        ///
        /// ```
        /// const [timestamp, latitude, longitude, accuracy, altitude, altitudeAccuracy] = jdunpack<[number, number, number, number, number, number]>(buf, "u64 i9.23 i9.23 u16.16 i26.6 u16.16")
        /// ```
        /// </summary>
        Position = 0x101,

        /// <summary>
        /// Read-write bool (uint8_t). Enables or disables the GPS module
        ///
        /// ```
        /// const [enabled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Enabled = 0x1,
    }

    public static class SatNavRegPack {
        /// <summary>
        /// Pack format for 'position' data.
        /// </summary>
        public const string Position = "u64 i9.23 i9.23 u16.16 i26.6 u16.16";

        /// <summary>
        /// Pack format for 'enabled' data.
        /// </summary>
        public const string Enabled = "u8";
    }

    public enum SatNavEvent : ushort {
        /// <summary>
        /// The module is disabled or lost connection with satellites.
        /// </summary>
        Inactive = 0x2,
    }

}
