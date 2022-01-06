namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint HidMouse = 0x1885dc1c;
    }

    [System.Flags]
    public enum HidMouseButton: ushort { // uint16_t
        Left = 0x1,
        Right = 0x2,
        Middle = 0x4,
    }


    public enum HidMouseButtonEvent: byte { // uint8_t
        Up = 0x1,
        Down = 0x2,
        Click = 0x3,
        DoubleClick = 0x4,
    }

    public enum HidMouseCmd {
        /// <summary>
        /// Sets the up/down state of one or more buttons.
        /// A `Click` is the same as `Down` followed by `Up` after 100ms.
        /// A `DoubleClick` is two clicks with `150ms` gap between them (that is, `100ms` first click, `150ms` gap, `100ms` second click).
        ///
        /// ```
        /// const [buttons, event] = jdunpack<[HidMouseButton, HidMouseButtonEvent]>(buf, "u16 u8")
        /// ```
        /// </summary>
        SetButton = 0x80,

        /// <summary>
        /// Moves the mouse by the distance specified.
        /// If the time is positive, it specifies how long to make the move.
        ///
        /// ```
        /// const [dx, dy, time] = jdunpack<[number, number, number]>(buf, "i16 i16 u16")
        /// ```
        /// </summary>
        Move = 0x81,

        /// <summary>
        /// Turns the wheel up or down. Positive if scrolling up.
        /// If the time is positive, it specifies how long to make the move.
        ///
        /// ```
        /// const [dy, time] = jdunpack<[number, number]>(buf, "i16 u16")
        /// ```
        /// </summary>
        Wheel = 0x82,
    }

    public static class HidMouseCmdPack {
        /// <summary>
        /// Pack format for 'set_button' register data.
        /// </summary>
        public const string SetButton = "u16 u8";

        /// <summary>
        /// Pack format for 'move' register data.
        /// </summary>
        public const string Move = "i16 i16 u16";

        /// <summary>
        /// Pack format for 'wheel' register data.
        /// </summary>
        public const string Wheel = "i16 u16";
    }

}
