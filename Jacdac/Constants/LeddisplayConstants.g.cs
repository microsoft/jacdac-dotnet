namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint LedDisplay = 0x1609d4f0;
        public const uint MaxPixelsLength = 0x40;
    }

    public enum LedDisplayLightType: byte { // uint8_t
        WS2812B_GRB = 0x0,
        APA102 = 0x10,
        SK9822 = 0x11,
    }


    public enum LedDisplayVariant: byte { // uint8_t
        Strip = 0x1,
        Ring = 0x2,
        Stick = 0x3,
        Jewel = 0x4,
        Matrix = 0x5,
    }

    public enum LedDisplayReg : ushort {
        /// <summary>
        /// Read-write bytes. A buffer of 24bit RGB color entries for each LED, in R, G, B order.
        ///
        /// ```
        /// const [pixels] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        Pixels = 0x2,

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
        /// Constant LightType (uint8_t). Specifies the type of light strip connected to controller.
        ///
        /// ```
        /// const [lightType] = jdunpack<[LedDisplayLightType]>(buf, "u8")
        /// ```
        /// </summary>
        LightType = 0x181,

        /// <summary>
        /// Constant # uint16_t. Specifies the number of pixels in the strip.
        ///
        /// ```
        /// const [numPixels] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        NumPixels = 0x182,

        /// <summary>
        /// Constant # uint16_t. If the LED pixel strip is a matrix, specifies the number of columns.
        ///
        /// ```
        /// const [numColumns] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        NumColumns = 0x183,

        /// <summary>
        /// Read-write mA uint16_t. Limit the power drawn by the light-strip (and controller).
        ///
        /// ```
        /// const [maxPower] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        MaxPower = 0x7,

        /// <summary>
        /// Constant Variant (uint8_t). Specifies the shape of the light strip.
        ///
        /// ```
        /// const [variant] = jdunpack<[LedDisplayVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class LedDisplayRegPack {
        /// <summary>
        /// Pack format for 'pixels' register data.
        /// </summary>
        public const string Pixels = "b";

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
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";
    }

}
