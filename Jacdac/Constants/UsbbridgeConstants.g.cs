namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint UsbBridge = 0x18f61a4a;
    }

    public enum UsbBridgeQByte: byte { // uint8_t
        Magic = 0xfe,
        LiteralMagic = 0xf8,
        Reserved = 0xf9,
        SerialGap = 0xfa,
        FrameGap = 0xfb,
        FrameStart = 0xfc,
        FrameEnd = 0xfd,
    }

    public enum UsbBridgeCmd : ushort {
        /// <summary>
        /// No args. Disables forwarding of Jacdac packets.
        /// </summary>
        DisablePackets = 0x80,

        /// <summary>
        /// No args. Enables forwarding of Jacdac packets.
        /// </summary>
        EnablePackets = 0x81,

        /// <summary>
        /// No args. Disables forwarding of serial log messages.
        /// </summary>
        DisableLog = 0x82,

        /// <summary>
        /// No args. Enables forwarding of serial log messages.
        /// </summary>
        EnableLog = 0x83,
    }

}
