namespace Jacdac {
    // Service: Flex
    public static class FlexConstants
    {
        public const uint ServiceClass = 0x1f47c6c6;
    }
    public enum FlexReg {
        /**
         * Read-only ratio u0.16 (uint16_t). The relative position of the slider.
         *
         * ```
         * const [bending] = jdunpack<[number]>(buf, "u0.16")
         * ```
         */
        Bending = 0x101,

        /**
         * Read-only ratio u0.16 (uint16_t). Absolute error on the reading value.
         *
         * ```
         * const [bendingError] = jdunpack<[number]>(buf, "u0.16")
         * ```
         */
        BendingError = 0x106,

        /**
         * Constant mm uint16_t. Length of the flex sensor
         *
         * ```
         * const [length] = jdunpack<[number]>(buf, "u16")
         * ```
         */
        Length = 0x180,
    }

}
