namespace Jacdac {
    // Service: Sound player
    public static class SoundPlayerConstants
    {
        public const uint ServiceClass = 0x1403d338;
    }
    public enum SoundPlayerReg {
        /**
         * Read-write ratio u0.16 (uint16_t). Global volume of the output. ``0`` means completely off. This volume is mixed with each play volumes.
         *
         * ```
         * const [volume] = jdunpack<[number]>(buf, "u0.16")
         * ```
         */
        Volume = 0x1,
    }

    public static class SoundPlayerRegPack {
        /**
         * Pack format for 'volume' register data.
         */
        public const string Volume = "u0.16";
    }

    public enum SoundPlayerCmd {
        /**
         * Argument: name string (bytes). Starts playing a sound.
         *
         * ```
         * const [name] = jdunpack<[string]>(buf, "s")
         * ```
         */
        Play = 0x80,

        /**
         * No args. Cancel any sound playing.
         */
        Cancel = 0x81,

        /**
         * Argument: sounds_port pipe (bytes). Returns the list of sounds available to play.
         *
         * ```
         * const [soundsPort] = jdunpack<[Uint8Array]>(buf, "b[12]")
         * ```
         */
        ListSounds = 0x82,
    }

    public static class SoundPlayerCmdPack {
        /**
         * Pack format for 'play' register data.
         */
        public const string Play = "s";

        /**
         * Pack format for 'list_sounds' register data.
         */
        public const string ListSounds = "b[12]";
    }


    /**
     * pipe_report ListSoundsPipe
     * ```
     * const [duration, name] = jdunpack<[number, string]>(buf, "u32 s")
     * ```
     */


    public static class SoundPlayerinfoPack {
        /**
         * Pack format for 'list_sounds_pipe' register data.
         */
        public const string ListSoundsPipe = "u32 s";
    }

}
