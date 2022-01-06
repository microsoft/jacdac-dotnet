namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint PulseOximeter = 0x10bb4eb6;
    }
    public enum PulseOximeterReg {
        /// <summary>
        /// Read-only % u8.8 (uint16_t). The estimated oxygen level in blood.
        ///
        /// ```
        /// const [oxygen] = jdunpack<[number]>(buf, "u8.8")
        /// ```
        /// </summary>
        Oxygen = 0x101,

        /// <summary>
        /// Read-only % u8.8 (uint16_t). The estimated error on the reported sensor data.
        ///
        /// ```
        /// const [oxygenError] = jdunpack<[number]>(buf, "u8.8")
        /// ```
        /// </summary>
        OxygenError = 0x106,
    }

    public static class PulseOximeterRegPack {
        /// <summary>
        /// Pack format for 'oxygen' register data.
        /// </summary>
        public const string Oxygen = "u8.8";

        /// <summary>
        /// Pack format for 'oxygen_error' register data.
        /// </summary>
        public const string OxygenError = "u8.8";
    }

}
