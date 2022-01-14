namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint CharacterScreen = 0x1f37c56a;
    }

    public enum CharacterScreenVariant: byte { // uint8_t
        LCD = 0x1,
        OLED = 0x2,
        Braille = 0x3,
    }


    public enum CharacterScreenTextDirection: byte { // uint8_t
        LeftToRight = 0x1,
        RightToLeft = 0x2,
    }

    public enum CharacterScreenReg : ushort {
        /// <summary>
        /// Read-write string (bytes). Text to show. Use `\n` to break lines.
        ///
        /// ```
        /// const [message] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        Message = 0x2,

        /// <summary>
        /// Read-write ratio u0.16 (uint16_t). Brightness of the screen. `0` means off.
        ///
        /// ```
        /// const [brightness] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        Brightness = 0x1,

        /// <summary>
        /// Constant Variant (uint8_t). Describes the type of character LED screen.
        ///
        /// ```
        /// const [variant] = jdunpack<[CharacterScreenVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,

        /// <summary>
        /// Read-write TextDirection (uint8_t). Specifies the RTL or LTR direction of the text.
        ///
        /// ```
        /// const [textDirection] = jdunpack<[CharacterScreenTextDirection]>(buf, "u8")
        /// ```
        /// </summary>
        TextDirection = 0x82,

        /// <summary>
        /// Constant # uint8_t. Gets the number of rows.
        ///
        /// ```
        /// const [rows] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Rows = 0x180,

        /// <summary>
        /// Constant # uint8_t. Gets the number of columns.
        ///
        /// ```
        /// const [columns] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Columns = 0x181,
    }

    public static class CharacterScreenRegPack {
        /// <summary>
        /// Pack format for 'message' register data.
        /// </summary>
        public const string Message = "s";

        /// <summary>
        /// Pack format for 'brightness' register data.
        /// </summary>
        public const string Brightness = "u0.16";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";

        /// <summary>
        /// Pack format for 'text_direction' register data.
        /// </summary>
        public const string TextDirection = "u8";

        /// <summary>
        /// Pack format for 'rows' register data.
        /// </summary>
        public const string Rows = "u8";

        /// <summary>
        /// Pack format for 'columns' register data.
        /// </summary>
        public const string Columns = "u8";
    }

}
