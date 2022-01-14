namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint LightBulb = 0x1cab054c;
    }
    public enum LightBulbReg : ushort {
        /// <summary>
        /// Read-write ratio u0.16 (uint16_t). Indicates the brightness of the light bulb. Zero means completely off and 0xffff means completely on.
        /// For non-dimmable lights, the value should be clamp to 0xffff for any non-zero value.
        ///
        /// ```
        /// const [brightness] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        Brightness = 0x1,

        /// <summary>
        /// Constant bool (uint8_t). Indicates if the light supports dimming.
        ///
        /// ```
        /// const [dimmable] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Dimmable = 0x180,
    }

    public static class LightBulbRegPack {
        /// <summary>
        /// Pack format for 'brightness' register data.
        /// </summary>
        public const string Brightness = "u0.16";

        /// <summary>
        /// Pack format for 'dimmable' register data.
        /// </summary>
        public const string Dimmable = "u8";
    }

}
