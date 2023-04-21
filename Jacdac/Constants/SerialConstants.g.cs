namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Serial = 0x11bae5c4;
    }

    public enum SerialParityType: byte { // uint8_t
        None = 0x0,
        Even = 0x1,
        Odd = 0x2,
    }

    public enum SerialReg : ushort {
        /// <summary>
        /// Read-write bool (uint8_t). Indicates if the serial connection is active.
        ///
        /// ```
        /// const [connected] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Connected = 0x1,

        /// <summary>
        /// Read-only string (bytes). User-friendly name of the connection.
        ///
        /// ```
        /// const [connectionName] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        ConnectionName = 0x181,

        /// <summary>
        /// Read-write baud uint32_t. A positive, non-zero value indicating the baud rate at which serial communication is be established.
        ///
        /// ```
        /// const [baudRate] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        BaudRate = 0x80,

        /// <summary>
        /// Read-write uint8_t. The number of data bits per frame. Either 7 or 8.
        ///
        /// ```
        /// const [dataBits] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        DataBits = 0x81,

        /// <summary>
        /// Read-write # uint8_t. The number of stop bits at the end of a frame. Either 1 or 2.
        ///
        /// ```
        /// const [stopBits] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        StopBits = 0x82,

        /// <summary>
        /// Read-write ParityType (uint8_t). The parity mode.
        ///
        /// ```
        /// const [parityMode] = jdunpack<[SerialParityType]>(buf, "u8")
        /// ```
        /// </summary>
        ParityMode = 0x83,

        /// <summary>
        /// Read-write # uint8_t. A positive, non-zero value indicating the size of the read and write buffers that should be created.
        ///
        /// ```
        /// const [bufferSize] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        BufferSize = 0x84,
    }

    public static class SerialRegPack {
        /// <summary>
        /// Pack format for 'connected' data.
        /// </summary>
        public const string Connected = "u8";

        /// <summary>
        /// Pack format for 'connection_name' data.
        /// </summary>
        public const string ConnectionName = "s";

        /// <summary>
        /// Pack format for 'baud_rate' data.
        /// </summary>
        public const string BaudRate = "u32";

        /// <summary>
        /// Pack format for 'data_bits' data.
        /// </summary>
        public const string DataBits = "u8";

        /// <summary>
        /// Pack format for 'stop_bits' data.
        /// </summary>
        public const string StopBits = "u8";

        /// <summary>
        /// Pack format for 'parity_mode' data.
        /// </summary>
        public const string ParityMode = "u8";

        /// <summary>
        /// Pack format for 'buffer_size' data.
        /// </summary>
        public const string BufferSize = "u8";
    }

    public enum SerialCmd : ushort {
        /// <summary>
        /// Argument: data bytes. Send a buffer of data over the serial transport.
        ///
        /// ```
        /// const [data] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        Send = 0x80,

        /// <summary>
        /// Argument: data bytes. Raised when a buffer of data is received.
        ///
        /// ```
        /// const [data] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        Received = 0x80,
    }

    public static class SerialCmdPack {
        /// <summary>
        /// Pack format for 'send' data.
        /// </summary>
        public const string Send = "b";

        /// <summary>
        /// Pack format for 'received' data.
        /// </summary>
        public const string Received = "b";
    }

}
