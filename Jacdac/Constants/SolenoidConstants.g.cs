namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Solenoid = 0x171723ca;
    }

    public enum SolenoidVariant: byte { // uint8_t
        PushPull = 0x1,
        Valve = 0x2,
        Latch = 0x3,
    }

    public enum SolenoidReg {
        /// <summary>
        /// Read-write bool (uint8_t). Indicates whether the solenoid is energized and pulled (on) or pushed (off).
        ///
        /// ```
        /// const [pulled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Pulled = 0x1,

        /// <summary>
        /// Constant Variant (uint8_t). Describes the type of solenoid used.
        ///
        /// ```
        /// const [variant] = jdunpack<[SolenoidVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class SolenoidRegPack {
        /// <summary>
        /// Pack format for 'pulled' register data.
        /// </summary>
        public const string Pulled = "u8";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";
    }

}
