namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Bootloader = 0x1ffa9948;
    }

    public enum BootloaderError: uint { // uint32_t
        NoError = 0x0,
        PacketTooSmall = 0x1,
        OutOfFlashableRange = 0x2,
        InvalidPageOffset = 0x3,
        NotPageAligned = 0x4,
    }

    public enum BootloaderCmd : ushort {
        /// <summary>
        /// No args. The `service_class` is always `0x1ffa9948`. The `product_identifier` identifies the kind of firmware
        /// that "fits" this device.
        /// </summary>
        Info = 0x0,

        /// <summary>
        /// report Info
        /// ```
        /// const [serviceClass, pageSize, flashableSize, productIdentifier] = jdunpack<[number, number, number, number]>(buf, "u32 u32 u32 u32")
        /// ```
        /// </summary>

        /// <summary>
        /// Argument: session_id uint32_t. The flashing server should generate a random id, and use this command to set it.
        ///
        /// ```
        /// const [sessionId] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        SetSession = 0x81,

        /// <summary>
        /// report SetSession
        /// ```
        /// const [sessionId] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>

        /// <summary>
        /// Use to send flashing data. A physical page is split into `chunk_max + 1` chunks, where `chunk_no = 0 ... chunk_max`.
        /// Each chunk is stored at `page_address + page_offset`. `page_address` has to be equal in all chunks,
        /// and is included in response.
        /// Only the last chunk causes writing to flash and elicits response.
        ///
        /// ```
        /// const [pageAddress, pageOffset, chunkNo, chunkMax, sessionId, pageData] = jdunpack<[number, number, number, number, number, Uint8Array]>(buf, "u32 u16 u8 u8 u32 x[4] x[4] x[4] x[4] b[208]")
        /// ```
        /// </summary>
        PageData = 0x80,

        /// <summary>
        /// report PageData
        /// ```
        /// const [sessionId, pageError, pageAddress] = jdunpack<[number, BootloaderError, number]>(buf, "u32 u32 u32")
        /// ```
        /// </summary>
    }

    public static class BootloaderCmdPack {
        /// <summary>
        /// Pack format for 'info' register data.
        /// </summary>
        public const string InfoReport = "u32 u32 u32 u32";

        /// <summary>
        /// Pack format for 'set_session' register data.
        /// </summary>
        public const string SetSession = "u32";

        /// <summary>
        /// Pack format for 'set_session' register data.
        /// </summary>
        public const string SetSessionReport = "u32";

        /// <summary>
        /// Pack format for 'page_data' register data.
        /// </summary>
        public const string PageData = "u32 u16 u8 u8 u32 u32 u32 u32 u32 b[208]";

        /// <summary>
        /// Pack format for 'page_data' register data.
        /// </summary>
        public const string PageDataReport = "u32 u32 u32";
    }

}
