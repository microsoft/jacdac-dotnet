namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint KeyboardClient = 0x113d023e;
    }
    public enum KeyboardClientEvent : ushort {
        /// <summary>
        /// Argument: key uint16_t. Emitted when a key is pressed.
        ///
        /// ```
        /// const [key] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        Down = 0x1,

        /// <summary>
        /// Argument: key uint16_t. Emitted when a key is held.
        ///
        /// ```
        /// const [key] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        Hold = 0x81,
    }

    public static class KeyboardClientEventPack {
        /// <summary>
        /// Pack format for 'down' data.
        /// </summary>
        public const string Down = "u16";

        /// <summary>
        /// Pack format for 'hold' data.
        /// </summary>
        public const string Hold = "u16";
    }

}
