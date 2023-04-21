namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint GPIO = 0x10d85a69;
    }

    public enum GPIOMode: byte { // uint8_t
        Off = 0x0,
        OffPullUp = 0x10,
        OffPullDown = 0x20,
        Input = 0x1,
        InputPullUp = 0x11,
        InputPullDown = 0x21,
        Output = 0x2,
        OutputHigh = 0x12,
        OutputLow = 0x22,
        AnalogIn = 0x3,
        Alternative = 0x4,
        BaseModeMask = 0xf,
    }


    [System.Flags]
    public enum GPIOCapabilities: ushort { // uint16_t
        PullUp = 0x1,
        PullDown = 0x2,
        Input = 0x4,
        Output = 0x8,
        Analog = 0x10,
    }

    public enum GPIOReg : ushort {
        /// <summary>
        /// Read-only digital_values bytes. For every pin set to `Input*` the corresponding **bit** in `digital_values` will be `1` if and only if
        /// the pin is high.
        /// For other pins, the bit is `0`.
        /// This is normally streamed at low-ish speed, but it's also automatically reported whenever
        /// a digital input pin changes value (throttled to ~100Hz).
        /// The analog values can be read with the `ADC` service.
        ///
        /// ```
        /// const [digitalValues] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        State = 0x101,

        /// <summary>
        /// Read-only # uint8_t. Number of pins that can be operated through this service.
        ///
        /// ```
        /// const [numPins] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        NumPins = 0x180,
    }

    public static class GPIORegPack {
        /// <summary>
        /// Pack format for 'state' data.
        /// </summary>
        public const string State = "b";

        /// <summary>
        /// Pack format for 'num_pins' data.
        /// </summary>
        public const string NumPins = "u8";
    }

    public enum GPIOCmd : ushort {
        /// <summary>
        /// Configure (including setting the value) zero or more pins.
        /// `Alternative` settings means the pin is controlled by other service (SPI, I2C, UART, PWM, etc.).
        ///
        /// ```
        /// const [rest] = jdunpack<[([number, GPIOMode])[]]>(buf, "r: u8 u8")
        /// const [pin, mode] = rest[0]
        /// ```
        /// </summary>
        Configure = 0x80,

        /// <summary>
        /// Argument: pin uint8_t. Report capabilities and name of a pin.
        ///
        /// ```
        /// const [pin] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        PinInfo = 0x81,

        /// <summary>
        /// report PinInfo
        /// ```
        /// const [pin, hwPin, capabilities, mode, label] = jdunpack<[number, number, GPIOCapabilities, GPIOMode, string]>(buf, "u8 u8 u16 u8 s")
        /// ```
        /// </summary>

        /// <summary>
        /// Argument: label string (bytes). This responds with `pin_info` report.
        ///
        /// ```
        /// const [label] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        PinByLabel = 0x83,

        /// <summary>
        /// report PinByLabel
        /// ```
        /// const [pin, hwPin, capabilities, mode, label] = jdunpack<[number, number, GPIOCapabilities, GPIOMode, string]>(buf, "u8 u8 u16 u8 s")
        /// ```
        /// </summary>

        /// <summary>
        /// Argument: hw_pin uint8_t. This responds with `pin_info` report.
        ///
        /// ```
        /// const [hwPin] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        PinByHwPin = 0x84,

        /// <summary>
        /// report PinByHwPin
        /// ```
        /// const [pin, hwPin, capabilities, mode, label] = jdunpack<[number, number, GPIOCapabilities, GPIOMode, string]>(buf, "u8 u8 u16 u8 s")
        /// ```
        /// </summary>
    }

    public static class GPIOCmdPack {
        /// <summary>
        /// Pack format for 'configure' data.
        /// </summary>
        public const string Configure = "r: u8 u8";

        /// <summary>
        /// Pack format for 'pin_info' data.
        /// </summary>
        public const string PinInfo = "u8";

        /// <summary>
        /// Pack format for 'pin_info' data.
        /// </summary>
        public const string PinInfoReport = "u8 u8 u16 u8 s";

        /// <summary>
        /// Pack format for 'pin_by_label' data.
        /// </summary>
        public const string PinByLabel = "s";

        /// <summary>
        /// Pack format for 'pin_by_label' data.
        /// </summary>
        public const string PinByLabelReport = "u8 u8 u16 u8 s";

        /// <summary>
        /// Pack format for 'pin_by_hw_pin' data.
        /// </summary>
        public const string PinByHwPin = "u8";

        /// <summary>
        /// Pack format for 'pin_by_hw_pin' data.
        /// </summary>
        public const string PinByHwPinReport = "u8 u8 u16 u8 s";
    }

}
