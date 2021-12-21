namespace Jacdac {
    // Service: Color
    public static class ColorConstants
    {
        public const uint ServiceClass = 0x1630d567;
    }
    public enum ColorReg {
        /**
         * Detected color in the RGB color space.
         *
         * ```
         * const [red, green, blue] = jdunpack<[number, number, number]>(buf, "u0.16 u0.16 u0.16")
         * ```
         */
        Color = 0x101,
    }

    public static class ColorRegPack {
        /**
         * Pack format for 'color' register data.
         */
        public const string Color = "u0.16 u0.16 u0.16";
    }

}
