namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Button = 0x1473a263;
    }
    public enum ButtonReg : ushort {
        /// <summary>
        /// Read-only ratio u0.16 (uint16_t). Indicates the pressure state of the button, where `0` is open.
        ///
        /// ```
        /// const [pressure] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        Pressure = 0x101,

        /// <summary>
        /// Constant bool (uint8_t). Indicates if the button provides analog `pressure` readings.
        ///
        /// ```
        /// const [analog] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Analog = 0x180,

        /// <summary>
        /// Read-only bool (uint8_t). Determines if the button is pressed currently.
        /// </summary>
        Pressed = 0x181,
    }

    public static class ButtonRegPack {
        /// <summary>
        /// Pack format for 'pressure' register data.
        /// </summary>
        public const string Pressure = "u0.16";

        /// <summary>
        /// Pack format for 'analog' register data.
        /// </summary>
        public const string Analog = "u8";

        /// <summary>
        /// Pack format for 'pressed' register data.
        /// </summary>
        public const string Pressed = "u8";
    }

    public enum ButtonEvent : ushort {
        /// <summary>
        /// Emitted when button goes from inactive to active.
        /// </summary>
        Down = 0x1,

        /// <summary>
        /// Argument: time ms uint32_t. Emitted when button goes from active to inactive. The 'time' parameter
        /// records the amount of time between the down and up events.
        ///
        /// ```
        /// const [time] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        Up = 0x2,

        /// <summary>
        /// Argument: time ms uint32_t. Emitted when the press time is greater than 500ms, and then at least every 500ms
        /// as long as the button remains pressed. The 'time' parameter records the the amount of time
        /// that the button has been held (since the down event).
        ///
        /// ```
        /// const [time] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        Hold = 0x81,
    }

    public static class ButtonEventPack {
        /// <summary>
        /// Pack format for 'up' register data.
        /// </summary>
        public const string Up = "u32";

        /// <summary>
        /// Pack format for 'hold' register data.
        /// </summary>
        public const string Hold = "u32";
    }

}
