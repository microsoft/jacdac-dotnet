namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Bridge = 0x1fe5b46f;
    }
    public enum BridgeReg : ushort {
        /// <summary>
        /// Read-write bool (uint8_t). Enables or disables the bridge.
        ///
        /// ```
        /// const [enabled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Enabled = 0x1,
    }

    public static class BridgeRegPack {
        /// <summary>
        /// Pack format for 'enabled' data.
        /// </summary>
        public const string Enabled = "u8";
    }

}
