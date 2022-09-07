namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint HidKeyboard = 0x18b05b6a;
    }

    public enum HidKeyboardSelector: ushort { // uint16_t
        None = 0x0,
        ErrorRollOver = 0x1,
        PostFail = 0x2,
        ErrorUndefined = 0x3,
        A = 0x4,
        B = 0x5,
        C = 0x6,
        D = 0x7,
        E = 0x8,
        F = 0x9,
        G = 0xa,
        H = 0xb,
        I = 0xc,
        J = 0xd,
        K = 0xe,
        L = 0xf,
        M = 0x10,
        N = 0x11,
        O = 0x12,
        P = 0x13,
        Q = 0x14,
        R = 0x15,
        S = 0x16,
        T = 0x17,
        U = 0x18,
        V = 0x19,
        W = 0x1a,
        X = 0x1b,
        Y = 0x1c,
        Z = 0x1d,
        _1 = 0x1e,
        _2 = 0x1f,
        _3 = 0x20,
        _4 = 0x21,
        _5 = 0x22,
        _6 = 0x23,
        _7 = 0x24,
        _8 = 0x25,
        _9 = 0x26,
        _0 = 0x27,
        Return = 0x28,
        Escape = 0x29,
        Backspace = 0x2a,
        Tab = 0x2b,
        Spacebar = 0x2c,
        Minus = 0x2d,
        Equals = 0x2e,
        LeftSquareBracket = 0x2f,
        RightSquareBracket = 0x30,
        Backslash = 0x31,
        NonUsHash = 0x32,
        Semicolon = 0x33,
        Quote = 0x34,
        GraveAccent = 0x35,
        Comma = 0x36,
        Period = 0x37,
        Slash = 0x38,
        CapsLock = 0x39,
        F1 = 0x3a,
        F2 = 0x3b,
        F3 = 0x3c,
        F4 = 0x3d,
        F5 = 0x3e,
        F6 = 0x3f,
        F7 = 0x40,
        F8 = 0x41,
        F9 = 0x42,
        F10 = 0x43,
        F11 = 0x44,
        F12 = 0x45,
        PrintScreen = 0x46,
        ScrollLock = 0x47,
        Pause = 0x48,
        Insert = 0x49,
        Home = 0x4a,
        PageUp = 0x4b,
        Delete = 0x4c,
        End = 0x4d,
        PageDown = 0x4e,
        RightArrow = 0x4f,
        LeftArrow = 0x50,
        DownArrow = 0x51,
        UpArrow = 0x52,
        KeypadNumLock = 0x53,
        KeypadDivide = 0x54,
        KeypadMultiply = 0x55,
        KeypadAdd = 0x56,
        KeypadSubtrace = 0x57,
        KeypadReturn = 0x58,
        Keypad1 = 0x59,
        Keypad2 = 0x5a,
        Keypad3 = 0x5b,
        Keypad4 = 0x5c,
        Keypad5 = 0x5d,
        Keypad6 = 0x5e,
        Keypad7 = 0x5f,
        Keypad8 = 0x60,
        Keypad9 = 0x61,
        Keypad0 = 0x62,
        KeypadDecimalPoint = 0x63,
        NonUsBackslash = 0x64,
        Application = 0x65,
        Power = 0x66,
        KeypadEquals = 0x67,
        F13 = 0x68,
        F14 = 0x69,
        F15 = 0x6a,
        F16 = 0x6b,
        F17 = 0x6c,
        F18 = 0x6d,
        F19 = 0x6e,
        F20 = 0x6f,
        F21 = 0x70,
        F22 = 0x71,
        F23 = 0x72,
        F24 = 0x73,
        Execute = 0x74,
        Help = 0x75,
        Menu = 0x76,
        Select = 0x77,
        Stop = 0x78,
        Again = 0x79,
        Undo = 0x7a,
        Cut = 0x7b,
        Copy = 0x7c,
        Paste = 0x7d,
        Find = 0x7e,
        Mute = 0x7f,
        VolumeUp = 0x80,
        VolumeDown = 0x81,
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

    public enum HidKeyboardCmd : ushort {
        /// <summary>
        /// Presses a key or a sequence of keys down.
        ///
        /// ```
        /// const [rest] = jdunpack<[([HidKeyboardSelector, HidKeyboardModifiers, HidKeyboardAction])[]]>(buf, "r: u16 u8 u8")
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
