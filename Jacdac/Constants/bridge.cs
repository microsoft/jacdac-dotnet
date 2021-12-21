namespace Jacdac {
    // Service: Bridge
    public static class BridgeConstants
    {
        public const uint ServiceClass = 0x1fe5b46f;
    }
    public enum BridgeReg {
        /**
         * Read-write bool (uint8_t). Enables or disables the bridge.
         *
         * ```
         * const [enabled] = jdunpack<[number]>(buf, "u8")
         * ```
         */
        Enabled = 0x1,
    }

    public static class BridgeRegPack {
        /**
         * Pack format for 'enabled' register data.
         */
        public const string Enabled = "u8";
    }

}
