namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Flex = 0x1f47c6c6;
    }
    public enum FlexReg : ushort {
        /// <summary>
        /// Read-only ratio i1.15 (int16_t). A measure of the bending.
        ///
        /// ```
        /// const [bending] = jdunpack<[number]>(buf, "i1.15")
        /// ```
        /// </summary>
        Bending = 0x101,

        /// <summary>
        /// Constant mm uint16_t. Length of the flex sensor
        ///
        /// ```
        /// const [length] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        Length = 0x180,
    }

    public static class FlexRegPack {
        /// <summary>
        /// Pack format for 'bending' data.
        /// </summary>
        public const string Bending = "i1.15";

        /// <summary>
        /// Pack format for 'length' data.
        /// </summary>
        public const string Length = "u16";
    }

}
