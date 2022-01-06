namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint HidKeyboard = 0x18b05b6a;
    }

    [System.Flags]
    public enum HidKeyboardModifiers: byte { // uint8_t
        None = 0x0,
        LeftControl = 0x1,
        LeftShift = 0x2,
        LeftAlt = 0x4,
        LeftGUI = 0x8,
        RightControl = 0x10,
        RightShift = 0x20,
        RightAlt = 0x40,
        RightGUI = 0x80,
    }


    public enum HidKeyboardAction: byte { // uint8_t
        Press = 0x0,
        Up = 0x1,
        Down = 0x2,
    }

    public enum HidKeyboardCmd {
        /// <summary>
        /// Presses a key or a sequence of keys down.
        ///
        /// ```
        /// const [rest] = jdunpack<[([number, HidKeyboardModifiers, HidKeyboardAction])[]]>(buf, "r: u16 u8 u8")
        /// const [selector, modifiers, action] = rest[0]
        /// ```
        /// </summary>
        Key = 0x80,

        /// <summary>
        /// No args. Clears all pressed keys.
        /// </summary>
        Clear = 0x81,
    }

    public static class HidKeyboardCmdPack {
        /// <summary>
        /// Pack format for 'key' register data.
        /// </summary>
        public const string Key = "r: u16 u8 u8";
    }

}
