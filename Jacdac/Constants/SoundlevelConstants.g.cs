namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint SoundLevel = 0x14ad1a5d;
    }
    public enum SoundLevelReg : ushort {
        /// <summary>
        /// Read-only ratio u0.16 (uint16_t). The sound level detected by the microphone
        ///
        /// ```
        /// const [soundLevel] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        SoundLevel = 0x101,

        /// <summary>
        /// Read-write bool (uint8_t). Turn on or off the microphone.
        ///
        /// ```
        /// const [enabled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Enabled = 0x1,
    }

    public static class SoundLevelRegPack {
        /// <summary>
        /// Pack format for 'sound_level' register data.
        /// </summary>
        public const string SoundLevel = "u0.16";

        /// <summary>
        /// Pack format for 'enabled' register data.
        /// </summary>
        public const string Enabled = "u8";
    }

}
