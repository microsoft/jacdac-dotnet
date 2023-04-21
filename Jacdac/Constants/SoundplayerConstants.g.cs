namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint SoundPlayer = 0x1403d338;
    }
    public enum SoundPlayerReg : ushort {
        /// <summary>
        /// Read-write ratio u0.16 (uint16_t). Global volume of the output. ``0`` means completely off. This volume is mixed with each play volumes.
        ///
        /// ```
        /// const [volume] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        Volume = 0x1,
    }

    public static class SoundPlayerRegPack {
        /// <summary>
        /// Pack format for 'volume' data.
        /// </summary>
        public const string Volume = "u0.16";
    }

    public enum SoundPlayerCmd : ushort {
        /// <summary>
        /// Argument: name string (bytes). Starts playing a sound.
        ///
        /// ```
        /// const [name] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        Play = 0x80,

        /// <summary>
        /// No args. Cancel any sound playing.
        /// </summary>
        Cancel = 0x81,

        /// <summary>
        /// Argument: sounds_port pipe (bytes). Returns the list of sounds available to play.
        ///
        /// ```
        /// const [soundsPort] = jdunpack<[Uint8Array]>(buf, "b[12]")
        /// ```
        /// </summary>
        ListSounds = 0x82,
    }

    public static class SoundPlayerCmdPack {
        /// <summary>
        /// Pack format for 'play' data.
        /// </summary>
        public const string Play = "s";

        /// <summary>
        /// Pack format for 'list_sounds' data.
        /// </summary>
        public const string ListSounds = "b[12]";
    }

    public enum SoundPlayerPipe : ushort {
        /// <summary>
        /// pipe_report ListSoundsPipe
        /// ```
        /// const [duration, name] = jdunpack<[number, string]>(buf, "u32 s")
        /// ```
        /// </summary>
    }

    public static class SoundPlayerPipePack {
        /// <summary>
        /// Pack format for 'list_sounds_pipe' data.
        /// </summary>
        public const string ListSoundsPipe = "u32 s";
    }

}
