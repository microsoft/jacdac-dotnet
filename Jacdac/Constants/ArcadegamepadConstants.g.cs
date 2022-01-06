namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint ArcadeGamepad = 0x1deaa06e;
    }

    public enum ArcadeGamepadButton: byte { // uint8_t
        Left = 0x1,
        Up = 0x2,
        Right = 0x3,
        Down = 0x4,
        A = 0x5,
        B = 0x6,
        Menu = 0x7,
        Select = 0x8,
        Reset = 0x9,
        Exit = 0xa,
    }

    public enum ArcadeGamepadReg {
        /// <summary>
        /// Indicates which buttons are currently active (pressed).
        /// `pressure` should be `0xff` for digital buttons, and proportional for analog ones.
        ///
        /// ```
        /// const [rest] = jdunpack<[([ArcadeGamepadButton, number])[]]>(buf, "r: u8 u0.8")
        /// const [button, pressure] = rest[0]
        /// ```
        /// </summary>
        Buttons = 0x101,

        /// <summary>
        /// Constant. Indicates number of players supported and which buttons are present on the controller.
        ///
        /// ```
        /// const [button] = jdunpack<[ArcadeGamepadButton[]]>(buf, "u8[]")
        /// ```
        /// </summary>
        AvailableButtons = 0x180,
    }

    public static class ArcadeGamepadRegPack {
        /// <summary>
        /// Pack format for 'buttons' register data.
        /// </summary>
        public const string Buttons = "r: u8 u0.8";

        /// <summary>
        /// Pack format for 'available_buttons' register data.
        /// </summary>
        public const string AvailableButtons = "r: u8";
    }

    public enum ArcadeGamepadEvent {
        /// <summary>
        /// Argument: button Button (uint8_t). Emitted when button goes from inactive to active.
        ///
        /// ```
        /// const [button] = jdunpack<[ArcadeGamepadButton]>(buf, "u8")
        /// ```
        /// </summary>
        Down = 0x1,

        /// <summary>
        /// Argument: button Button (uint8_t). Emitted when button goes from active to inactive.
        ///
        /// ```
        /// const [button] = jdunpack<[ArcadeGamepadButton]>(buf, "u8")
        /// ```
        /// </summary>
        Up = 0x2,
    }

    public static class ArcadeGamepadEventPack {
        /// <summary>
        /// Pack format for 'down' register data.
        /// </summary>
        public const string Down = "u8";

        /// <summary>
        /// Pack format for 'up' register data.
        /// </summary>
        public const string Up = "u8";
    }

}
