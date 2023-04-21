namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint HeartRate = 0x166c6dc4;
    }

    public enum HeartRateVariant: byte { // uint8_t
        Finger = 0x1,
        Chest = 0x2,
        Wrist = 0x3,
        Pump = 0x4,
        WebCam = 0x5,
    }

    public enum HeartRateReg : ushort {
        /// <summary>
        /// Read-only bpm u16.16 (uint32_t). The estimated heart rate.
        ///
        /// ```
        /// const [heartRate] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        HeartRate = 0x101,

        /// <summary>
        /// Read-only bpm u16.16 (uint32_t). The estimated error on the reported sensor data.
        ///
        /// ```
        /// const [heartRateError] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        HeartRateError = 0x106,

        /// <summary>
        /// Constant Variant (uint8_t). The type of physical sensor
        ///
        /// ```
        /// const [variant] = jdunpack<[HeartRateVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class HeartRateRegPack {
        /// <summary>
        /// Pack format for 'heart_rate' data.
        /// </summary>
        public const string HeartRate = "u16.16";

        /// <summary>
        /// Pack format for 'heart_rate_error' data.
        /// </summary>
        public const string HeartRateError = "u16.16";

        /// <summary>
        /// Pack format for 'variant' data.
        /// </summary>
        public const string Variant = "u8";
    }

}
