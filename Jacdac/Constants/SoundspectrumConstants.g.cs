namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint SoundSpectrum = 0x157abc1e;
    }
    public enum SoundSpectrumReg {
        /// <summary>
        /// Read-only bytes. The computed frequency data.
        ///
        /// ```
        /// const [frequencyBins] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        FrequencyBins = 0x101,

        /// <summary>
        /// Read-write bool (uint8_t). Turns on/off the micropohone.
        ///
        /// ```
        /// const [enabled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Enabled = 0x1,

        /// <summary>
        /// Read-write uint8_t. The power of 2 used as the size of the FFT to be used to determine the frequency domain.
        ///
        /// ```
        /// const [fftPow2Size] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        FftPow2Size = 0x80,

        /// <summary>
        /// Read-write dB int16_t. The minimum power value in the scaling range for the FFT analysis data
        ///
        /// ```
        /// const [minDecibels] = jdunpack<[number]>(buf, "i16")
        /// ```
        /// </summary>
        MinDecibels = 0x81,

        /// <summary>
        /// Read-write dB int16_t. The maximum power value in the scaling range for the FFT analysis data
        ///
        /// ```
        /// const [maxDecibels] = jdunpack<[number]>(buf, "i16")
        /// ```
        /// </summary>
        MaxDecibels = 0x82,

        /// <summary>
        /// Read-write ratio u0.8 (uint8_t). The averaging constant with the last analysis frame.
        /// If `0` is set, there is no averaging done, whereas a value of `1` means "overlap the previous and current buffer quite a lot while computing the value".
        ///
        /// ```
        /// const [smoothingTimeConstant] = jdunpack<[number]>(buf, "u0.8")
        /// ```
        /// </summary>
        SmoothingTimeConstant = 0x83,
    }

    public static class SoundSpectrumRegPack {
        /// <summary>
        /// Pack format for 'frequency_bins' register data.
        /// </summary>
        public const string FrequencyBins = "b";

        /// <summary>
        /// Pack format for 'enabled' register data.
        /// </summary>
        public const string Enabled = "u8";

        /// <summary>
        /// Pack format for 'fft_pow2_size' register data.
        /// </summary>
        public const string FftPow2Size = "u8";

        /// <summary>
        /// Pack format for 'min_decibels' register data.
        /// </summary>
        public const string MinDecibels = "i16";

        /// <summary>
        /// Pack format for 'max_decibels' register data.
        /// </summary>
        public const string MaxDecibels = "i16";

        /// <summary>
        /// Pack format for 'smoothing_time_constant' register data.
        /// </summary>
        public const string SmoothingTimeConstant = "u0.8";
    }

}
