namespace Jacdac {
    // Service: Flex
    public static class FlexConstants
    {
        public const uint ServiceClass = 0x1f47c6c6;
    }
    public enum FlexReg {
        /**
         * Read-only ratio i1.15 (int16_t). A measure of the bending.
         *
         * ```
         * const [bending] = jdunpack<[number]>(buf, "i1.15")
         * ```
         */
        Bending = 0x101,

        /**
         * Constant mm uint16_t. Length of the flex sensor
         *
         * ```
         * const [length] = jdunpack<[number]>(buf, "u16")
         * ```
         */
        Length = 0x180,
    }

    public static class FlexRegPack {
        /**
         * Pack format for 'bending' register data.
         */
        public const string Bending = "i1.15";

        /**
         * Pack format for 'length' register data.
         */
        public const string Length = "u16";
    }

}
