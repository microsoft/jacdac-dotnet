namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint SensorAggregator = 0x1d90e1c5;
    }

    public enum SensorAggregatorSampleType: byte { // uint8_t
        U8 = 0x8,
        I8 = 0x88,
        U16 = 0x10,
        I16 = 0x90,
        U32 = 0x20,
        I32 = 0xa0,
    }

    public enum SensorAggregatorReg : ushort {
        /// <summary>
        /// Set automatic input collection.
        /// These settings are stored in flash.
        ///
        /// ```
        /// const [samplingInterval, samplesInWindow, rest] = jdunpack<[number, number, ([Uint8Array, number, number, number, SensorAggregatorSampleType, number])[]]>(buf, "u16 u16 x[4] r: b[8] u32 u8 u8 u8 i8")
        /// const [deviceId, serviceClass, serviceNum, sampleSize, sampleType, sampleShift] = rest[0]
        /// ```
        /// </summary>
        Inputs = 0x80,

        /// <summary>
        /// Read-only uint32_t. Number of input samples collected so far.
        ///
        /// ```
        /// const [numSamples] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        NumSamples = 0x180,

        /// <summary>
        /// Read-only B uint8_t. Size of a single sample.
        ///
        /// ```
        /// const [sampleSize] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        SampleSize = 0x181,

        /// <summary>
        /// Read-write # uint32_t. When set to `N`, will stream `N` samples as `current_sample` reading.
        ///
        /// ```
        /// const [streamingSamples] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        StreamingSamples = 0x81,

        /// <summary>
        /// Read-only bytes. Last collected sample.
        ///
        /// ```
        /// const [currentSample] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        CurrentSample = 0x101,
    }

    public static class SensorAggregatorRegPack {
        /// <summary>
        /// Pack format for 'inputs' register data.
        /// </summary>
        public const string Inputs = "u16 u16 u32 r: b[8] u32 u8 u8 u8 i8";

        /// <summary>
        /// Pack format for 'num_samples' register data.
        /// </summary>
        public const string NumSamples = "u32";

        /// <summary>
        /// Pack format for 'sample_size' register data.
        /// </summary>
        public const string SampleSize = "u8";

        /// <summary>
        /// Pack format for 'streaming_samples' register data.
        /// </summary>
        public const string StreamingSamples = "u32";

        /// <summary>
        /// Pack format for 'current_sample' register data.
        /// </summary>
        public const string CurrentSample = "b";
    }

}
