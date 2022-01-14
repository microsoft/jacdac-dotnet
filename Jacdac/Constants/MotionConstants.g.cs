namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Motion = 0x1179a749;
    }

    public enum MotionVariant: byte { // uint8_t
        PIR = 0x1,
    }

    public enum MotionReg : ushort {
        /// <summary>
        /// Read-only bool (uint8_t). Reports is movement is currently detected by the sensor.
        ///
        /// ```
        /// const [moving] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Moving = 0x101,

        /// <summary>
        /// Constant m u16.16 (uint32_t). Maximum distance where objects can be detected.
        ///
        /// ```
        /// const [maxDistance] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        MaxDistance = 0x180,

        /// <summary>
        /// Constant ° uint16_t. Opening of the field of view
        ///
        /// ```
        /// const [angle] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        Angle = 0x181,

        /// <summary>
        /// Constant Variant (uint8_t). Type of physical sensor
        ///
        /// ```
        /// const [variant] = jdunpack<[MotionVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class MotionRegPack {
        /// <summary>
        /// Pack format for 'moving' register data.
        /// </summary>
        public const string Moving = "u8";

        /// <summary>
        /// Pack format for 'max_distance' register data.
        /// </summary>
        public const string MaxDistance = "u16.16";

        /// <summary>
        /// Pack format for 'angle' register data.
        /// </summary>
        public const string Angle = "u16";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";
    }

    public enum MotionEvent : ushort {
        /// <summary>
        /// A movement was detected.
        /// </summary>
        Movement = 0x1,
    }

}
