namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint HidJoystick = 0x1a112155;
    }
    public enum HidJoystickReg : ushort {
        /// <summary>
        /// Constant uint8_t. Number of button report supported
        ///
        /// ```
        /// const [buttonCount] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        ButtonCount = 0x180,

        /// <summary>
        /// Constant uint32_t. A bitset that indicates which button is analog.
        ///
        /// ```
        /// const [buttonsAnalog] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        ButtonsAnalog = 0x181,

        /// <summary>
        /// Constant uint8_t. Number of analog input supported
        ///
        /// ```
        /// const [axisCount] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        AxisCount = 0x182,
    }

    public static class HidJoystickRegPack {
        /// <summary>
        /// Pack format for 'button_count' register data.
        /// </summary>
        public const string ButtonCount = "u8";

        /// <summary>
        /// Pack format for 'buttons_analog' register data.
        /// </summary>
        public const string ButtonsAnalog = "u32";

        /// <summary>
        /// Pack format for 'axis_count' register data.
        /// </summary>
        public const string AxisCount = "u8";
    }

    public enum HidJoystickCmd : ushort {
        /// <summary>
        /// Sets the up/down button state, one byte per button, supports analog buttons. For digital buttons, use `0` for released, `1` for pressed.
        ///
        /// ```
        /// const [pressure] = jdunpack<[number[]]>(buf, "u0.8[]")
        /// ```
        /// </summary>
        SetButtons = 0x80,

        /// <summary>
        /// Sets the state of analog inputs.
        ///
        /// ```
        /// const [position] = jdunpack<[number[]]>(buf, "i1.15[]")
        /// ```
        /// </summary>
        SetAxis = 0x81,
    }

    public static class HidJoystickCmdPack {
        /// <summary>
        /// Pack format for 'set_buttons' register data.
        /// </summary>
        public const string SetButtons = "r: u0.8";

        /// <summary>
        /// Pack format for 'set_axis' register data.
        /// </summary>
        public const string SetAxis = "r: i1.15";
    }

}
