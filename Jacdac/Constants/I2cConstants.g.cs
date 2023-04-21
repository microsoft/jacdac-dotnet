namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint I2C = 0x1c18ca43;
    }

    public enum I2CStatus: byte { // uint8_t
        OK = 0x0,
        NAckAddr = 0x1,
        NAckData = 0x2,
        NoI2C = 0x3,
    }

    public enum I2CReg : ushort {
        /// <summary>
        /// Read-only bool (uint8_t). Indicates whether the I2C is working.
        ///
        /// ```
        /// const [ok] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Ok = 0x180,
    }

    public static class I2CRegPack {
        /// <summary>
        /// Pack format for 'ok' data.
        /// </summary>
        public const string Ok = "u8";
    }

    public enum I2CCmd : ushort {
        /// <summary>
        /// `address` is 7-bit.
        /// `num_read` can be 0 if nothing needs to be read.
        /// The `write_buf` includes the register address if required (first one or two bytes).
        ///
        /// ```
        /// const [address, numRead, writeBuf] = jdunpack<[number, number, Uint8Array]>(buf, "u8 u8 b")
        /// ```
        /// </summary>
        Transaction = 0x80,

        /// <summary>
        /// report Transaction
        /// ```
        /// const [status, readBuf] = jdunpack<[I2CStatus, Uint8Array]>(buf, "u8 b")
        /// ```
        /// </summary>
    }

    public static class I2CCmdPack {
        /// <summary>
        /// Pack format for 'transaction' data.
        /// </summary>
        public const string Transaction = "u8 u8 b";

        /// <summary>
        /// Pack format for 'transaction' data.
        /// </summary>
        public const string TransactionReport = "u8 b";
    }

}
