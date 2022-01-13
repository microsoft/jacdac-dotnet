namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint BrailleDisplay = 0x13bfb7cc;
    }
    public enum BrailleDisplayReg : ushort {
        /// <summary>
        /// Read-write bool (uint8_t). Determins if the braille display is active.
        ///
        /// ```
        /// const [enabled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Enabled = 0x1,

        /// <summary>
        /// Read-write string (bytes). Braille patterns to show. Must be unicode characters between `0x2800` and `0x28ff`.
        ///
        /// ```
        /// const [patterns] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        Patterns = 0x2,

        /// <summary>
        /// Constant # uint8_t. Gets the number of patterns that can be displayed.
        ///
        /// ```
        /// const [length] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Length = 0x181,
    }

    public static class BrailleDisplayRegPack {
        /// <summary>
        /// Pack format for 'enabled' register data.
        /// </summary>
        public const string Enabled = "u8";

        /// <summary>
        /// Pack format for 'patterns' register data.
        /// </summary>
        public const string Patterns = "s";

        /// <summary>
        /// Pack format for 'length' register data.
        /// </summary>
        public const string Length = "u8";
    }

}
