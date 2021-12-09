namespace Jacdac {
    // Service: Illuminance
    public static class IlluminanceConstants
    {
        public const uint ServiceClass = 0x1e6ecaf2;
    }
    public enum IlluminanceReg {
        /**
         * Read-only lux u22.10 (uint32_t). The amount of illuminance, as lumens per square metre.
         *
         * ```
         * const [illuminance] = jdunpack<[number]>(buf, "u22.10")
         * ```
         */
        Illuminance = 0x101,

        /**
         * Read-only lux u22.10 (uint32_t). Error on the reported sensor value.
         *
         * ```
         * const [illuminanceError] = jdunpack<[number]>(buf, "u22.10")
         * ```
         */
        IlluminanceError = 0x106,
    }

}
