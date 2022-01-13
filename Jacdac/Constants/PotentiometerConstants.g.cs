namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Potentiometer = 0x1f274746;
    }

    public enum PotentiometerVariant: byte { // uint8_t
        Slider = 0x1,
        Rotary = 0x2,
    }

    public enum PotentiometerReg : ushort {
        /// <summary>
        /// Read-only ratio u0.16 (uint16_t). The relative position of the slider.
        ///
        /// ```
        /// const [position] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        Position = 0x101,

        /// <summary>
        /// Constant Variant (uint8_t). Specifies the physical layout of the potentiometer.
        ///
        /// ```
        /// const [variant] = jdunpack<[PotentiometerVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class PotentiometerRegPack {
        /// <summary>
        /// Pack format for 'position' register data.
        /// </summary>
        public const string Position = "u0.16";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";
    }

}
