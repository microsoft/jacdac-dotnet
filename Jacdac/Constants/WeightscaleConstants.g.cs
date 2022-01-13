namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint WeightScale = 0x1f4d5040;
    }

    public enum WeightScaleVariant: byte { // uint8_t
        Body = 0x1,
        Food = 0x2,
        Jewelry = 0x3,
    }

    public enum WeightScaleReg : ushort {
        /// <summary>
        /// Read-only kg u16.16 (uint32_t). The reported weight.
        ///
        /// ```
        /// const [weight] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        Weight = 0x101,

        /// <summary>
        /// Read-only kg u16.16 (uint32_t). The estimate error on the reported reading.
        ///
        /// ```
        /// const [weightError] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        WeightError = 0x106,

        /// <summary>
        /// Read-write kg u16.16 (uint32_t). Calibrated zero offset error on the scale, i.e. the measured weight when nothing is on the scale.
        /// You do not need to subtract that from the reading, it has already been done.
        ///
        /// ```
        /// const [zeroOffset] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        ZeroOffset = 0x80,

        /// <summary>
        /// Read-write u16.16 (uint32_t). Calibrated gain on the weight scale error.
        ///
        /// ```
        /// const [gain] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        Gain = 0x81,

        /// <summary>
        /// Constant kg u16.16 (uint32_t). Maximum supported weight on the scale.
        ///
        /// ```
        /// const [maxWeight] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        MaxWeight = 0x105,

        /// <summary>
        /// Constant kg u16.16 (uint32_t). Minimum recommend weight on the scale.
        ///
        /// ```
        /// const [minWeight] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        MinWeight = 0x104,

        /// <summary>
        /// Constant kg u16.16 (uint32_t). Smallest, yet distinguishable change in reading.
        ///
        /// ```
        /// const [weightResolution] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        WeightResolution = 0x108,

        /// <summary>
        /// Constant Variant (uint8_t). The type of physical scale
        ///
        /// ```
        /// const [variant] = jdunpack<[WeightScaleVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class WeightScaleRegPack {
        /// <summary>
        /// Pack format for 'weight' register data.
        /// </summary>
        public const string Weight = "u16.16";

        /// <summary>
        /// Pack format for 'weight_error' register data.
        /// </summary>
        public const string WeightError = "u16.16";

        /// <summary>
        /// Pack format for 'zero_offset' register data.
        /// </summary>
        public const string ZeroOffset = "u16.16";

        /// <summary>
        /// Pack format for 'gain' register data.
        /// </summary>
        public const string Gain = "u16.16";

        /// <summary>
        /// Pack format for 'max_weight' register data.
        /// </summary>
        public const string MaxWeight = "u16.16";

        /// <summary>
        /// Pack format for 'min_weight' register data.
        /// </summary>
        public const string MinWeight = "u16.16";

        /// <summary>
        /// Pack format for 'weight_resolution' register data.
        /// </summary>
        public const string WeightResolution = "u16.16";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";
    }

    public enum WeightScaleCmd : ushort {
        /// <summary>
        /// No args. Call this command when there is nothing on the scale. If supported, the module should save the calibration data.
        /// </summary>
        CalibrateZeroOffset = 0x80,

        /// <summary>
        /// Argument: weight g u22.10 (uint32_t). Call this command with the weight of the thing on the scale.
        ///
        /// ```
        /// const [weight] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        CalibrateGain = 0x81,
    }

    public static class WeightScaleCmdPack {
        /// <summary>
        /// Pack format for 'calibrate_gain' register data.
        /// </summary>
        public const string CalibrateGain = "u22.10";
    }

}
