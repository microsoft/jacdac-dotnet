namespace Jacdac {
    public static partial class ServiceClasses
    {
    }
    public enum SensorReg : ushort {
        /// <summary>
        /// Read-write # uint8_t. Asks device to stream a given number of samples
        /// (clients will typically write `255` to this register every second or so, while streaming is required).
        ///
        /// ```
        /// const [streamingSamples] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        StreamingSamples = 0x3,

        /// <summary>
        /// Read-write ms uint32_t. Period between packets of data when streaming in milliseconds.
        ///
        /// ```
        /// const [streamingInterval] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        StreamingInterval = 0x4,

        /// <summary>
        /// Constant ms uint32_t. Preferred default streaming interval for sensor in milliseconds.
        ///
        /// ```
        /// const [streamingPreferredInterval] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        StreamingPreferredInterval = 0x102,
    }

    public static class SensorRegPack {
        /// <summary>
        /// Pack format for 'streaming_samples' register data.
        /// </summary>
        public const string StreamingSamples = "u8";

        /// <summary>
        /// Pack format for 'streaming_interval' register data.
        /// </summary>
        public const string StreamingInterval = "u32";

        /// <summary>
        /// Pack format for 'streaming_preferred_interval' register data.
        /// </summary>
        public const string StreamingPreferredInterval = "u32";
    }

}
