namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint LedStrip = 0x126f00e0;
    }

    public enum LedStripLightType: byte { // uint8_t
        WS2812B_GRB = 0x0,
        APA102 = 0x10,
        SK9822 = 0x11,
    }


    public enum LedStripVariant: byte { // uint8_t
        Strip = 0x1,
        Ring = 0x2,
        Stick = 0x3,
        Jewel = 0x4,
        Matrix = 0x5,
    }

    public enum LedStripReg : ushort {
        /// <summary>
        /// Read-write ratio u0.8 (uint8_t). Set the luminosity of the strip.
        /// At `0` the power to the strip is completely shut down.
        ///
        /// ```
        /// const [brightness] = jdunpack<[number]>(buf, "u0.8")
        /// ```
        /// </summary>
        Brightness = 0x1,

        /// <summary>
        /// Read-only ratio u0.8 (uint8_t). This is the luminosity actually applied to the strip.
        /// May be lower than `brightness` if power-limited by the `max_power` register.
        /// It will rise slowly (few seconds) back to `brightness` is limits are no longer required.
        ///
        /// ```
        /// const [actualBrightness] = jdunpack<[number]>(buf, "u0.8")
        /// ```
        /// </summary>
        ActualBrightness = 0x180,

        /// <summary>
        /// Read-write LightType (uint8_t). Specifies the type of light strip connected to controller.
        /// Controllers which are sold with lights should default to the correct type
        /// and could not allow change.
        ///
        /// ```
        /// const [lightType] = jdunpack<[LedStripLightType]>(buf, "u8")
        /// ```
        /// </summary>
        LightType = 0x80,

        /// <summary>
        /// Read-write # uint16_t. Specifies the number of pixels in the strip.
        /// Controllers which are sold with lights should default to the correct length
        /// and could not allow change. Increasing length at runtime leads to ineffective use of memory and may lead to controller reboot.
        ///
        /// ```
        /// const [numPixels] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        NumPixels = 0x81,

        /// <summary>
        /// Read-write # uint16_t. If the LED pixel strip is a matrix, specifies the number of columns. Otherwise, a square shape is assumed. Controllers which are sold with lights should default to the correct length
        /// and could not allow change. Increasing length at runtime leads to ineffective use of memory and may lead to controller reboot.
        ///
        /// ```
        /// const [numColumns] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        NumColumns = 0x83,

        /// <summary>
        /// Read-write mA uint16_t. Limit the power drawn by the light-strip (and controller).
        ///
        /// ```
        /// const [maxPower] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        MaxPower = 0x7,

        /// <summary>
        /// Constant # uint16_t. The maximum supported number of pixels.
        /// All writes to `num_pixels` are clamped to `max_pixels`.
        ///
        /// ```
        /// const [maxPixels] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        MaxPixels = 0x181,

        /// <summary>
        /// Read-write # uint16_t. How many times to repeat the program passed in `run` command.
        /// Should be set before the `run` command.
        /// Setting to `0` means to repeat forever.
        ///
        /// ```
        /// const [numRepeats] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        NumRepeats = 0x82,

        /// <summary>
        /// Constant Variant (uint8_t). Specifies the shape of the light strip.
        ///
        /// ```
        /// const [variant] = jdunpack<[LedStripVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class LedStripRegPack {
        /// <summary>
        /// Pack format for 'brightness' register data.
        /// </summary>
        public const string Brightness = "u0.8";

        /// <summary>
        /// Pack format for 'actual_brightness' register data.
        /// </summary>
        public const string ActualBrightness = "u0.8";

        /// <summary>
        /// Pack format for 'light_type' register data.
        /// </summary>
        public const string LightType = "u8";

        /// <summary>
        /// Pack format for 'num_pixels' register data.
        /// </summary>
        public const string NumPixels = "u16";

        /// <summary>
        /// Pack format for 'num_columns' register data.
        /// </summary>
        public const string NumColumns = "u16";

        /// <summary>
        /// Pack format for 'max_power' register data.
        /// </summary>
        public const string MaxPower = "u16";

        /// <summary>
        /// Pack format for 'max_pixels' register data.
        /// </summary>
        public const string MaxPixels = "u16";

        /// <summary>
        /// Pack format for 'num_repeats' register data.
        /// </summary>
        public const string NumRepeats = "u16";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";
    }

    public enum LedStripCmd : ushort {
        /// <summary>
        /// Argument: program bytes. Run the given light "program". See service description for details.
        ///
        /// ```
        /// const [program] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        Run = 0x81,
    }

    public static class LedStripCmdPack {
        /// <summary>
        /// Pack format for 'run' register data.
        /// </summary>
        public const string Run = "b";
    }

}
