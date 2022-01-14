namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Dmx = 0x11cf8c05;
    }
    public enum DmxReg : ushort {
        /// <summary>
        /// Read-write bool (uint8_t). Determines if the DMX bridge is active.
        ///
        /// ```
        /// const [enabled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Enabled = 0x1,
    }

    public static class DmxRegPack {
        /// <summary>
        /// Pack format for 'enabled' register data.
        /// </summary>
        public const string Enabled = "u8";
    }

    public enum DmxCmd : ushort {
        /// <summary>
        /// Argument: channels bytes. Send a DMX packet, up to 236bytes long, including the start code.
        ///
        /// ```
        /// const [channels] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        Send = 0x80,
    }

    public static class DmxCmdPack {
        /// <summary>
        /// Pack format for 'send' register data.
        /// </summary>
        public const string Send = "b";
    }

}
