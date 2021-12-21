namespace Jacdac {
    // Service: Character Screen
    public static class CharacterScreenConstants
    {
        public const uint ServiceClass = 0x1f37c56a;
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

    public enum CharacterScreenReg {
        /**
         * Read-write string (bytes). Text to show. Use `\n` to break lines.
         *
         * ```
         * const [message] = jdunpack<[string]>(buf, "s")
         * ```
         */
        Message = 0x2,

        /**
         * Read-write ratio u0.16 (uint16_t). Brightness of the screen. `0` means off.
         *
         * ```
         * const [brightness] = jdunpack<[number]>(buf, "u0.16")
         * ```
         */
        Brightness = 0x1,

        /**
         * Constant Variant (uint8_t). Describes the type of character LED screen.
         *
         * ```
         * const [variant] = jdunpack<[CharacterScreenVariant]>(buf, "u8")
         * ```
         */
        Variant = 0x107,

        /**
         * Read-write TextDirection (uint8_t). Specifies the RTL or LTR direction of the text.
         *
         * ```
         * const [textDirection] = jdunpack<[CharacterScreenTextDirection]>(buf, "u8")
         * ```
         */
        TextDirection = 0x82,

        /**
         * Constant # uint8_t. Gets the number of rows.
         *
         * ```
         * const [rows] = jdunpack<[number]>(buf, "u8")
         * ```
         */
        Rows = 0x180,

        /**
         * Constant # uint8_t. Gets the number of columns.
         *
         * ```
         * const [columns] = jdunpack<[number]>(buf, "u8")
         * ```
         */
        Columns = 0x181,
    }

    public static class CharacterScreenRegPack {
        /**
         * Pack format for 'message' register data.
         */
        public const string Message = "s";

        /**
         * Pack format for 'brightness' register data.
         */
        public const string Brightness = "u0.16";

        /**
         * Pack format for 'variant' register data.
         */
        public const string Variant = "u8";

        /**
         * Pack format for 'text_direction' register data.
         */
        public const string TextDirection = "u8";

        /**
         * Pack format for 'rows' register data.
         */
        public const string Rows = "u8";

        /**
         * Pack format for 'columns' register data.
         */
        public const string Columns = "u8";
    }

}
