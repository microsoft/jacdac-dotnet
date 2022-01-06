namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint IndexedScreen = 0x16fa36e5;
    }
    public enum IndexedScreenCmd {
        /// <summary>
        /// Sets the update window for subsequent `set_pixels` commands.
        ///
        /// ```
        /// const [x, y, width, height] = jdunpack<[number, number, number, number]>(buf, "u16 u16 u16 u16")
        /// ```
        /// </summary>
        StartUpdate = 0x81,

        /// <summary>
        /// Argument: pixels bytes. Set pixels in current window, according to current palette.
        /// Each "line" of data is aligned to a byte.
        ///
        /// ```
        /// const [pixels] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        SetPixels = 0x83,
    }

    public static class IndexedScreenCmdPack {
        /// <summary>
        /// Pack format for 'start_update' register data.
        /// </summary>
        public const string StartUpdate = "u16 u16 u16 u16";

        /// <summary>
        /// Pack format for 'set_pixels' register data.
        /// </summary>
        public const string SetPixels = "b";
    }

    public enum IndexedScreenReg {
        /// <summary>
        /// Read-write ratio u0.8 (uint8_t). Set backlight brightness.
        /// If set to `0` the display may go to sleep.
        ///
        /// ```
        /// const [brightness] = jdunpack<[number]>(buf, "u0.8")
        /// ```
        /// </summary>
        Brightness = 0x1,

        /// <summary>
        /// The current palette.
        /// The color entry repeats `1 << bits_per_pixel` times.
        /// This register may be write-only.
        ///
        /// ```
        /// const [rest] = jdunpack<[([number, number, number])[]]>(buf, "r: u8 u8 u8 x[1]")
        /// const [blue, green, red] = rest[0]
        /// ```
        /// </summary>
        Palette = 0x80,

        /// <summary>
        /// Constant bit uint8_t. Determines the number of palette entries.
        /// Typical values are 1, 2, 4, or 8.
        ///
        /// ```
        /// const [bitsPerPixel] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        BitsPerPixel = 0x180,

        /// <summary>
        /// Constant px uint16_t. Screen width in "natural" orientation.
        ///
        /// ```
        /// const [width] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        Width = 0x181,

        /// <summary>
        /// Constant px uint16_t. Screen height in "natural" orientation.
        ///
        /// ```
        /// const [height] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        Height = 0x182,

        /// <summary>
        /// Read-write bool (uint8_t). If true, consecutive pixels in the "width" direction are sent next to each other (this is typical for graphics cards).
        /// If false, consecutive pixels in the "height" direction are sent next to each other.
        /// For embedded screen controllers, this is typically true iff `width < height`
        /// (in other words, it's only true for portrait orientation screens).
        /// Some controllers may allow the user to change this (though the refresh order may not be optimal then).
        /// This is independent of the `rotation` register.
        ///
        /// ```
        /// const [widthMajor] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        WidthMajor = 0x81,

        /// <summary>
        /// Read-write px uint8_t. Every pixel sent over wire is represented by `up_sampling x up_sampling` square of physical pixels.
        /// Some displays may allow changing this (which will also result in changes to `width` and `height`).
        /// Typical values are 1 and 2.
        ///
        /// ```
        /// const [upSampling] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        UpSampling = 0x82,

        /// <summary>
        /// Read-write ° uint16_t. Possible values are 0, 90, 180 and 270 only.
        /// Write to this register do not affect `width` and `height` registers,
        /// and may be ignored by some screens.
        ///
        /// ```
        /// const [rotation] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        Rotation = 0x83,
    }

    public static class IndexedScreenRegPack {
        /// <summary>
        /// Pack format for 'brightness' register data.
        /// </summary>
        public const string Brightness = "u0.8";

        /// <summary>
        /// Pack format for 'palette' register data.
        /// </summary>
        public const string Palette = "r: u8 u8 u8 u8";

        /// <summary>
        /// Pack format for 'bits_per_pixel' register data.
        /// </summary>
        public const string BitsPerPixel = "u8";

        /// <summary>
        /// Pack format for 'width' register data.
        /// </summary>
        public const string Width = "u16";

        /// <summary>
        /// Pack format for 'height' register data.
        /// </summary>
        public const string Height = "u16";

        /// <summary>
        /// Pack format for 'width_major' register data.
        /// </summary>
        public const string WidthMajor = "u8";

        /// <summary>
        /// Pack format for 'up_sampling' register data.
        /// </summary>
        public const string UpSampling = "u8";

        /// <summary>
        /// Pack format for 'rotation' register data.
        /// </summary>
        public const string Rotation = "u16";
    }

}
