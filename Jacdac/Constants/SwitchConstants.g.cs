namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Switch = 0x1ad29402;
    }

    public enum SwitchVariant: byte { // uint8_t
        Slide = 0x1,
        Tilt = 0x2,
        PushButton = 0x3,
        Tactile = 0x4,
        Toggle = 0x5,
        Proximity = 0x6,
        Magnetic = 0x7,
        FootPedal = 0x8,
    }

    public enum SwitchReg : ushort {
        /// <summary>
        /// Read-only bool (uint8_t). Indicates whether the switch is currently active (on).
        ///
        /// ```
        /// const [active] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Active = 0x101,

        /// <summary>
        /// Constant Variant (uint8_t). Describes the type of switch used.
        ///
        /// ```
        /// const [variant] = jdunpack<[SwitchVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,

        /// <summary>
        /// Constant s u22.10 (uint32_t). Specifies the delay without activity to automatically turn off after turning on.
        /// For example, some light switches in staircases have such a capability.
        ///
        /// ```
        /// const [autoOffDelay] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        AutoOffDelay = 0x180,
    }

    public static class SwitchRegPack {
        /// <summary>
        /// Pack format for 'active' register data.
        /// </summary>
        public const string Active = "u8";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";

        /// <summary>
        /// Pack format for 'auto_off_delay' register data.
        /// </summary>
        public const string AutoOffDelay = "u22.10";
    }

    public enum SwitchEvent : ushort {
        /// <summary>
        /// Emitted when switch goes from `off` to `on`.
        /// </summary>
        On = 0x1,

        /// <summary>
        /// Emitted when switch goes from `on` to `off`.
        /// </summary>
        Off = 0x2,
    }

}
