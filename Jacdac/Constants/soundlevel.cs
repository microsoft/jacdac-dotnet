namespace Jacdac {
    // Service: Sound level
    public static class SoundLevelConstants
    {
        public const uint ServiceClass = 0x14ad1a5d;
    }
    public enum SoundLevelReg {
        /**
         * Read-only ratio u0.16 (uint16_t). The sound level detected by the microphone
         *
         * ```
         * const [soundLevel] = jdunpack<[number]>(buf, "u0.16")
         * ```
         */
        SoundLevel = 0x101,

        /**
         * Read-write bool (uint8_t). Turn on or off the microphone.
         *
         * ```
         * const [enabled] = jdunpack<[number]>(buf, "u8")
         * ```
         */
        Enabled = 0x1,

        /**
         * Read-write ratio u0.16 (uint16_t). The sound level to trigger a loud event.
         *
         * ```
         * const [loudThreshold] = jdunpack<[number]>(buf, "u0.16")
         * ```
         */
        LoudThreshold = 0x6,

        /**
         * Read-write ratio u0.16 (uint16_t). The sound level to trigger a quiet event.
         *
         * ```
         * const [quietThreshold] = jdunpack<[number]>(buf, "u0.16")
         * ```
         */
        QuietThreshold = 0x5,
    }

    public static class SoundLevelRegPack {
        /**
         * Pack format for 'sound_level' register data.
         */
        public const string SoundLevel = "u0.16";

        /**
         * Pack format for 'enabled' register data.
         */
        public const string Enabled = "u8";

        /**
         * Pack format for 'loud_threshold' register data.
         */
        public const string LoudThreshold = "u0.16";

        /**
         * Pack format for 'quiet_threshold' register data.
         */
        public const string QuietThreshold = "u0.16";
    }

    public enum SoundLevelEvent {
        /**
         * Raised when a loud sound is detected
         */
        Loud = 0x1,

        /**
         * Raised when a period of quietness is detected
         */
        Quiet = 0x2,
    }

}
