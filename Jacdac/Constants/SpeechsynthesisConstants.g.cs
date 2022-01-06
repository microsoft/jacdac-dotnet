namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint SpeechSynthesis = 0x1204d995;
    }
    public enum SpeechSynthesisReg {
        /// <summary>
        /// Read-write bool (uint8_t). Determines if the speech engine is in a non-paused state.
        ///
        /// ```
        /// const [enabled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Enabled = 0x1,

        /// <summary>
        /// Read-write string (bytes). Language used for utterances as defined in https://www.ietf.org/rfc/bcp/bcp47.txt.
        ///
        /// ```
        /// const [lang] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        Lang = 0x80,

        /// <summary>
        /// Read-write ratio u0.8 (uint8_t). Volume for utterances.
        ///
        /// ```
        /// const [volume] = jdunpack<[number]>(buf, "u0.8")
        /// ```
        /// </summary>
        Volume = 0x81,

        /// <summary>
        /// Read-write u16.16 (uint32_t). Pitch for utterances
        ///
        /// ```
        /// const [pitch] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        Pitch = 0x82,

        /// <summary>
        /// Read-write u16.16 (uint32_t). Rate for utterances
        ///
        /// ```
        /// const [rate] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        Rate = 0x83,
    }

    public static class SpeechSynthesisRegPack {
        /// <summary>
        /// Pack format for 'enabled' register data.
        /// </summary>
        public const string Enabled = "u8";

        /// <summary>
        /// Pack format for 'lang' register data.
        /// </summary>
        public const string Lang = "s";

        /// <summary>
        /// Pack format for 'volume' register data.
        /// </summary>
        public const string Volume = "u0.8";

        /// <summary>
        /// Pack format for 'pitch' register data.
        /// </summary>
        public const string Pitch = "u16.16";

        /// <summary>
        /// Pack format for 'rate' register data.
        /// </summary>
        public const string Rate = "u16.16";
    }

    public enum SpeechSynthesisCmd {
        /// <summary>
        /// Argument: text string (bytes). Adds an utterance to the utterance queue; it will be spoken when any other utterances queued before it have been spoken.
        ///
        /// ```
        /// const [text] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        Speak = 0x80,

        /// <summary>
        /// No args. Cancels current utterance and all utterances from the utterance queue.
        /// </summary>
        Cancel = 0x81,
    }

    public static class SpeechSynthesisCmdPack {
        /// <summary>
        /// Pack format for 'speak' register data.
        /// </summary>
        public const string Speak = "s";
    }

}
