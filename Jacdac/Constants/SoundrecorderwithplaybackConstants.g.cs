namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint SoundRecorderWithPlayback = 0x1b72bf50;
    }

    public enum SoundRecorderWithPlaybackStatus: byte { // uint8_t
        Idle = 0x0,
        Recording = 0x1,
        Playing = 0x2,
    }

    public enum SoundRecorderWithPlaybackCmd {
        /// <summary>
        /// No args. Replay recorded audio.
        /// </summary>
        Play = 0x80,

        /// <summary>
        /// Argument: duration ms uint16_t. Record audio for N milliseconds.
        ///
        /// ```
        /// const [duration] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        Record = 0x81,

        /// <summary>
        /// No args. Cancel record, the `time` register will be updated by already cached data.
        /// </summary>
        Cancel = 0x82,
    }

    public static class SoundRecorderWithPlaybackCmdPack {
        /// <summary>
        /// Pack format for 'record' register data.
        /// </summary>
        public const string Record = "u16";
    }

    public enum SoundRecorderWithPlaybackReg {
        /// <summary>
        /// Read-only Status (uint8_t). Indicate the current status
        ///
        /// ```
        /// const [status] = jdunpack<[SoundRecorderWithPlaybackStatus]>(buf, "u8")
        /// ```
        /// </summary>
        Status = 0x180,

        /// <summary>
        /// Read-only ms uint16_t. Milliseconds of audio recorded.
        ///
        /// ```
        /// const [time] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        Time = 0x181,

        /// <summary>
        /// Read-write ratio u0.8 (uint8_t). Playback volume control
        ///
        /// ```
        /// const [volume] = jdunpack<[number]>(buf, "u0.8")
        /// ```
        /// </summary>
        Volume = 0x1,
    }

    public static class SoundRecorderWithPlaybackRegPack {
        /// <summary>
        /// Pack format for 'status' register data.
        /// </summary>
        public const string Status = "u8";

        /// <summary>
        /// Pack format for 'time' register data.
        /// </summary>
        public const string Time = "u16";

        /// <summary>
        /// Pack format for 'volume' register data.
        /// </summary>
        public const string Volume = "u0.8";
    }

}
