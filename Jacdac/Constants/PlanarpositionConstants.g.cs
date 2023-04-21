namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint PlanarPosition = 0x1dc37f55;
    }

    public enum PlanarPositionVariant: byte { // uint8_t
        OpticalMousePosition = 0x1,
    }

    public enum PlanarPositionReg : ushort {
        /// <summary>
        /// The current position of the sensor.
        ///
        /// ```
        /// const [x, y] = jdunpack<[number, number]>(buf, "i22.10 i22.10")
        /// ```
        /// </summary>
        Position = 0x101,

        /// <summary>
        /// Constant Variant (uint8_t). Specifies the type of physical sensor.
        ///
        /// ```
        /// const [variant] = jdunpack<[PlanarPositionVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class PlanarPositionRegPack {
        /// <summary>
        /// Pack format for 'position' data.
        /// </summary>
        public const string Position = "i22.10 i22.10";

        /// <summary>
        /// Pack format for 'variant' data.
        /// </summary>
        public const string Variant = "u8";
    }

}
