namespace Jacdac {
    // Service: Pulse Oximeter
    public static class PulseOximeterConstants
    {
        public const uint ServiceClass = 0x10bb4eb6;
    }
    public enum PulseOximeterReg {
        /**
         * Read-only % u8.8 (uint16_t). The estimated oxygen level in blood.
         *
         * ```
         * const [oxygen] = jdunpack<[number]>(buf, "u8.8")
         * ```
         */
        Oxygen = 0x101,

        /**
         * Read-only % u8.8 (uint16_t). The estimated error on the reported sensor data.
         *
         * ```
         * const [oxygenError] = jdunpack<[number]>(buf, "u8.8")
         * ```
         */
        OxygenError = 0x106,
    }

    public static class PulseOximeterRegPack {
        /**
         * Pack format for 'oxygen' register data.
         */
        public const string Oxygen = "u8.8";

        /**
         * Pack format for 'oxygen_error' register data.
         */
        public const string OxygenError = "u8.8";
    }

}
