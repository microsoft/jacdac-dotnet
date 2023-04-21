namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Color = 0x1630d567;
    }
    public enum ColorReg : ushort {
        /// <summary>
        /// Detected color in the RGB color space.
        ///
        /// ```
        /// const [red, green, blue] = jdunpack<[number, number, number]>(buf, "u0.16 u0.16 u0.16")
        /// ```
        /// </summary>
        Color = 0x101,
    }

    public static class ColorRegPack {
        /// <summary>
        /// Pack format for 'color' data.
        /// </summary>
        public const string Color = "u0.16 u0.16 u0.16";
    }

}
