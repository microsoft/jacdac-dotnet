namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint DotMatrix = 0x110d154b;
    }

    public enum DotMatrixVariant: byte { // uint8_t
        LED = 0x1,
        Braille = 0x2,
    }

    public enum DotMatrixReg {
        /// <summary>
        /// Read-write bytes. The state of the screen where dot on/off state is
        /// stored as a bit, column by column. The column should be byte aligned.
        ///
        /// ```
        /// const [dots] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        Dots = 0x2,

        /// <summary>
        /// Read-write ratio u0.8 (uint8_t). Reads the general brightness of the display, brightness for LEDs. `0` when the screen is off.
        ///
        /// ```
        /// const [brightness] = jdunpack<[number]>(buf, "u0.8")
        /// ```
        /// </summary>
        Brightness = 0x1,

        /// <summary>
        /// Constant # uint16_t. Number of rows on the screen
        ///
        /// ```
        /// const [rows] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        Rows = 0x181,

        /// <summary>
        /// Constant # uint16_t. Number of columns on the screen
        ///
        /// ```
        /// const [columns] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        Columns = 0x182,

        /// <summary>
        /// Constant Variant (uint8_t). Describes the type of matrix used.
        ///
        /// ```
        /// const [variant] = jdunpack<[DotMatrixVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class DotMatrixRegPack {
        /// <summary>
        /// Pack format for 'dots' register data.
        /// </summary>
        public const string Dots = "b";

        /// <summary>
        /// Pack format for 'brightness' register data.
        /// </summary>
        public const string Brightness = "u0.8";

        /// <summary>
        /// Pack format for 'rows' register data.
        /// </summary>
        public const string Rows = "u16";

        /// <summary>
        /// Pack format for 'columns' register data.
        /// </summary>
        public const string Columns = "u16";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";
    }

}
