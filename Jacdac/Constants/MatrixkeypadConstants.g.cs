namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint MatrixKeypad = 0x13062dc8;
    }

    public enum MatrixKeypadVariant: byte { // uint8_t
        Membrane = 0x1,
        Keyboard = 0x2,
        Elastomer = 0x3,
        ElastomerLEDPixel = 0x4,
    }

    public enum MatrixKeypadReg {
        /// <summary>
        /// Read-only. The coordinate of the button currently pressed. Keys are zero-indexed from left to right, top to bottom:
        /// ``row = index / columns``, ``column = index % columns``.
        ///
        /// ```
        /// const [index] = jdunpack<[number[]]>(buf, "u8[]")
        /// ```
        /// </summary>
        Pressed = 0x101,

        /// <summary>
        /// Constant # uint8_t. Number of rows in the matrix
        ///
        /// ```
        /// const [rows] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Rows = 0x180,

        /// <summary>
        /// Constant # uint8_t. Number of columns in the matrix
        ///
        /// ```
        /// const [columns] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Columns = 0x181,

        /// <summary>
        /// Constant. The characters printed on the keys if any, in indexing sequence.
        ///
        /// ```
        /// const [label] = jdunpack<[string[]]>(buf, "z[]")
        /// ```
        /// </summary>
        Labels = 0x182,

        /// <summary>
        /// Constant Variant (uint8_t). The type of physical keypad. If the variant is ``ElastomerLEDPixel``
        /// and the next service on the device is a ``LEDPixel`` service, it is considered
        /// as the service controlling the LED pixel on the keypad.
        ///
        /// ```
        /// const [variant] = jdunpack<[MatrixKeypadVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class MatrixKeypadRegPack {
        /// <summary>
        /// Pack format for 'pressed' register data.
        /// </summary>
        public const string Pressed = "r: u8";

        /// <summary>
        /// Pack format for 'rows' register data.
        /// </summary>
        public const string Rows = "u8";

        /// <summary>
        /// Pack format for 'columns' register data.
        /// </summary>
        public const string Columns = "u8";

        /// <summary>
        /// Pack format for 'labels' register data.
        /// </summary>
        public const string Labels = "r: z";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";
    }

    public enum MatrixKeypadEvent {
        /// <summary>
        /// Argument: uint8_t. Emitted when a key, at the given index, goes from inactive (`pressed == 0`) to active.
        ///
        /// ```
        /// const [down] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Down = 0x1,

        /// <summary>
        /// Argument: uint8_t. Emitted when a key, at the given index, goes from active (`pressed == 1`) to inactive.
        ///
        /// ```
        /// const [up] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Up = 0x2,

        /// <summary>
        /// Argument: uint8_t. Emitted together with `up` when the press time was not longer than 500ms.
        ///
        /// ```
        /// const [click] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Click = 0x80,

        /// <summary>
        /// Argument: uint8_t. Emitted together with `up` when the press time was more than 500ms.
        ///
        /// ```
        /// const [longClick] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        LongClick = 0x81,
    }

    public static class MatrixKeypadEventPack {
        /// <summary>
        /// Pack format for 'down' register data.
        /// </summary>
        public const string Down = "u8";

        /// <summary>
        /// Pack format for 'up' register data.
        /// </summary>
        public const string Up = "u8";

        /// <summary>
        /// Pack format for 'click' register data.
        /// </summary>
        public const string Click = "u8";

        /// <summary>
        /// Pack format for 'long_click' register data.
        /// </summary>
        public const string LongClick = "u8";
    }

}
