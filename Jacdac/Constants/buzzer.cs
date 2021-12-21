namespace Jacdac {
    // Service: Buzzer
    public static class BuzzerConstants
    {
        public const uint ServiceClass = 0x1b57b1d7;
    }
    public enum BuzzerReg {
        /**
         * Read-write ratio u0.8 (uint8_t). The volume (duty cycle) of the buzzer.
         *
         * ```
         * const [volume] = jdunpack<[number]>(buf, "u0.8")
         * ```
         */
        Volume = 0x1,
    }

    public static class BuzzerRegPack {
        /**
         * Pack format for 'volume' register data.
         */
        public const string Volume = "u0.8";
    }

    public enum BuzzerCmd {
        /**
         * Play a PWM tone with given period and duty for given duration.
         * The duty is scaled down with `volume` register.
         * To play tone at frequency `F` Hz and volume `V` (in `0..1`) you will want
         * to send `P = 1000000 / F` and `D = P * V / 2`.
         *
         * ```
         * const [period, duty, duration] = jdunpack<[number, number, number]>(buf, "u16 u16 u16")
         * ```
         */
        PlayTone = 0x80,

        /**
         * Play a note at the given frequency and volume.
         */
        PlayNote = 0x81,
    }

    public static class BuzzerCmdPack {
        /**
         * Pack format for 'play_tone' register data.
         */
        public const string PlayTone = "u16 u16 u16";

        /**
         * Pack format for 'play_note' register data.
         */
        public const string PlayNote = "u16 u0.16 u16";
    }

}
