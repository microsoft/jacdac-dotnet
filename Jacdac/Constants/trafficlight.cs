namespace Jacdac {
    // Service: Traffic Light
    public static class TrafficLightConstants
    {
        public const uint ServiceClass = 0x15c38d9b;
    }
    public enum TrafficLightReg {
        /**
         * Read-write bool (uint8_t). The on/off state of the red light.
         *
         * ```
         * const [red] = jdunpack<[number]>(buf, "u8")
         * ```
         */
        Red = 0x80,

        /**
         * Read-write bool (uint8_t). The on/off state of the red light.
         *
         * ```
         * const [orange] = jdunpack<[number]>(buf, "u8")
         * ```
         */
        Orange = 0x81,

        /**
         * Read-write bool (uint8_t). The on/off state of the red light.
         *
         * ```
         * const [green] = jdunpack<[number]>(buf, "u8")
         * ```
         */
        Green = 0x82,
    }

    public static class TrafficLightRegPack {
        /**
         * Pack format for 'red' register data.
         */
        public const string Red = "u8";

        /**
         * Pack format for 'orange' register data.
         */
        public const string Orange = "u8";

        /**
         * Pack format for 'green' register data.
         */
        public const string Green = "u8";
    }

}
