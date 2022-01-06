namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Buzzer = 0x1b57b1d7;
    }
    public enum BuzzerReg {
        /// <summary>
        /// Read-write ratio u0.8 (uint8_t). The volume (duty cycle) of the buzzer.
        ///
        /// ```
        /// const [volume] = jdunpack<[number]>(buf, "u0.8")
        /// ```
        /// </summary>
        Volume = 0x1,
    }

    public static class BuzzerRegPack {
        /// <summary>
        /// Pack format for 'volume' register data.
        /// </summary>
        public const string Volume = "u0.8";
    }

    public enum BuzzerCmd {
        /// <summary>
        /// Play a PWM tone with given period and duty for given duration.
        /// The duty is scaled down with `volume` register.
        /// To play tone at frequency `F` Hz and volume `V` (in `0..1`) you will want
        /// to send `P = 1000000 / F` and `D = P * V / 2`.
        ///
        /// ```
        /// const [period, duty, duration] = jdunpack<[number, number, number]>(buf, "u16 u16 u16")
        /// ```
        /// </summary>
        PlayTone = 0x80,

        /// <summary>
        /// Play a note at the given frequency and volume.
        /// </summary>
        PlayNote = 0x81,
    }

    public static class BuzzerCmdPack {
        /// <summary>
        /// Pack format for 'play_tone' register data.
        /// </summary>
        public const string PlayTone = "u16 u16 u16";

        /// <summary>
        /// Pack format for 'play_note' register data.
        /// </summary>
        public const string PlayNote = "u16 u0.16 u16";
    }

}
