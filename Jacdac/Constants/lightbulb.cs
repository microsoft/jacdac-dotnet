namespace Jacdac {
    // Service: Light bulb
    public static class LightBulbConstants
    {
        public const uint ServiceClass = 0x1cab054c;
    }
    public enum LightBulbReg {
        /**
         * Read-write ratio u0.16 (uint16_t). Indicates the brightness of the light bulb. Zero means completely off and 0xffff means completely on.
         * For non-dimmable lights, the value should be clamp to 0xffff for any non-zero value.
         *
         * ```
         * const [brightness] = jdunpack<[number]>(buf, "u0.16")
         * ```
         */
        Brightness = 0x1,

        /**
         * Constant bool (uint8_t). Indicates if the light supports dimming.
         *
         * ```
         * const [dimmable] = jdunpack<[number]>(buf, "u8")
         * ```
         */
        Dimmable = 0x180,
    }

    public static class LightBulbRegPack {
        /**
         * Pack format for 'brightness' register data.
         */
        public const string Brightness = "u0.16";

        /**
         * Pack format for 'dimmable' register data.
         */
        public const string Dimmable = "u8";
    }

}
