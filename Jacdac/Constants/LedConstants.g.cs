namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Led = 0x1609d4f0;
        public const uint LedMaxPixelsLength = 0x40;
    }

    public enum LedVariant: byte { // uint8_t
        Strip = 0x1,
        Ring = 0x2,
        Stick = 0x3,
        Jewel = 0x4,
        Matrix = 0x5,
    }

    public enum LedReg : ushort {
        /// <summary>
        /// Read-write bytes. A buffer of 24bit RGB color entries for each LED, in R, G, B order.
        /// When writing, if the buffer is too short, the remaining pixels are set to `#000000`;
        /// if the buffer is too long, the write may be ignored, or the additional pixels may be ignored.
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
        /// Constant # uint16_t. If known, specifies the number of LEDs in parallel on this device.
        /// The actual number of LEDs is `num_pixels * leds_per_pixel`.
        ///
        /// ```
        /// const [ledsPerPixel] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        LedsPerPixel = 0x184,

        /// <summary>
        /// Constant nm uint16_t. If monochrome LED, specifies the wave length of the LED.
        /// Register is missing for RGB LEDs.
        ///
        /// ```
        /// const [waveLength] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        WaveLength = 0x185,

        /// <summary>
        /// Constant mcd uint16_t. The luminous intensity of all the LEDs, at full brightness, in micro candella.
        ///
        /// ```
        /// const [luminousIntensity] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        LuminousIntensity = 0x186,

        /// <summary>
        /// Constant Variant (uint8_t). Specifies the shape of the light strip.
        ///
        /// ```
        /// const [variant] = jdunpack<[LedVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class LedRegPack {
        /// <summary>
        /// Pack format for 'pixels' data.
        /// </summary>
        public const string Pixels = "b";

        /// <summary>
        /// Pack format for 'brightness' data.
        /// </summary>
        public const string Brightness = "u0.8";

        /// <summary>
        /// Pack format for 'actual_brightness' data.
        /// </summary>
        public const string ActualBrightness = "u0.8";

        /// <summary>
        /// Pack format for 'num_pixels' data.
        /// </summary>
        public const string NumPixels = "u16";

        /// <summary>
        /// Pack format for 'num_columns' data.
        /// </summary>
        public const string NumColumns = "u16";

        /// <summary>
        /// Pack format for 'max_power' data.
        /// </summary>
        public const string MaxPower = "u16";

        /// <summary>
        /// Pack format for 'leds_per_pixel' data.
        /// </summary>
        public const string LedsPerPixel = "u16";

        /// <summary>
        /// Pack format for 'wave_length' data.
        /// </summary>
        public const string WaveLength = "u16";

        /// <summary>
        /// Pack format for 'luminous_intensity' data.
        /// </summary>
        public const string LuminousIntensity = "u16";

        /// <summary>
        /// Pack format for 'variant' data.
        /// </summary>
        public const string Variant = "u8";
    }

}
