namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint ArcadeSound = 0x1fc63606;
    }
    public enum ArcadeSoundCmd : ushort {
        /// <summary>
        /// Argument: samples bytes. Play samples, which are single channel, signed 16-bit little endian values.
        ///
        /// ```
        /// const [samples] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        Play = 0x80,
    }

    public static class ArcadeSoundCmdPack {
        /// <summary>
        /// Pack format for 'play' data.
        /// </summary>
        public const string Play = "b";
    }

    public enum ArcadeSoundReg : ushort {
        /// <summary>
        /// Read-write Hz u22.10 (uint32_t). Get or set playback sample rate (in samples per second).
        /// If you set it, read it back, as the value may be rounded up or down.
        ///
        /// ```
        /// const [sampleRate] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        SampleRate = 0x80,

        /// <summary>
        /// Constant B uint32_t. The size of the internal audio buffer.
        ///
        /// ```
        /// const [bufferSize] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        BufferSize = 0x180,

        /// <summary>
        /// Read-only B uint32_t. How much data is still left in the buffer to play.
        /// Clients should not send more data than `buffer_size - buffer_pending`,
        /// but can keep the `buffer_pending` as low as they want to ensure low latency
        /// of audio playback.
        ///
        /// ```
        /// const [bufferPending] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        BufferPending = 0x181,
    }

    public static class ArcadeSoundRegPack {
        /// <summary>
        /// Pack format for 'sample_rate' data.
        /// </summary>
        public const string SampleRate = "u22.10";

        /// <summary>
        /// Pack format for 'buffer_size' data.
        /// </summary>
        public const string BufferSize = "u32";

        /// <summary>
        /// Pack format for 'buffer_pending' data.
        /// </summary>
        public const string BufferPending = "u32";
    }

}
