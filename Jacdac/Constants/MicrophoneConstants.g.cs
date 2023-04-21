namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Microphone = 0x113dac86;
    }
    public enum MicrophoneCmd : ushort {
        /// <summary>
        /// The samples will be streamed back over the `samples` pipe.
        /// If `num_samples` is `0`, streaming will only stop when the pipe is closed.
        /// Otherwise the specified number of samples is streamed.
        /// Samples are sent as `i16`.
        ///
        /// ```
        /// const [samples, numSamples] = jdunpack<[Uint8Array, number]>(buf, "b[12] u32")
        /// ```
        /// </summary>
        Sample = 0x81,
    }

    public static class MicrophoneCmdPack {
        /// <summary>
        /// Pack format for 'sample' data.
        /// </summary>
        public const string Sample = "b[12] u32";
    }

    public enum MicrophoneReg : ushort {
        /// <summary>
        /// Read-write μs uint32_t. Get or set microphone sampling period.
        /// Sampling rate is `1_000_000 / sampling_period Hz`.
        ///
        /// ```
        /// const [samplingPeriod] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        SamplingPeriod = 0x80,
    }

    public static class MicrophoneRegPack {
        /// <summary>
        /// Pack format for 'sampling_period' data.
        /// </summary>
        public const string SamplingPeriod = "u32";
    }

}
