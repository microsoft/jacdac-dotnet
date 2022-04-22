namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint MagneticFieldLevel = 0x12fe180f;
    }

    public enum MagneticFieldLevelVariant: byte { // uint8_t
        AnalogNS = 0x1,
        AnalogN = 0x2,
        AnalogS = 0x3,
        DigitalNS = 0x4,
        DigitalN = 0x5,
        DigitalS = 0x6,
    }

    public enum MagneticFieldLevelReg : ushort {
        /// <summary>
        /// Read-only ratio i1.15 (int16_t). Indicates the strength of magnetic field between -1 and 1.
        /// When no magnet is present the value should be around 0.
        /// For analog sensors,
        /// when the north pole of the magnet is on top of the module
        /// and closer than south pole, then the value should be positive.
        /// For digital sensors,
        /// the value should either `0` or `1`, regardless of polarity.
        ///
        /// ```
        /// const [strength] = jdunpack<[number]>(buf, "i1.15")
        /// ```
        /// </summary>
        Strength = 0x101,

        /// <summary>
        /// Read-only bool (uint8_t). Determines if the magnetic field is present.
        /// If the event `active` is observed, `detected` is true; if `inactive` is observed, `detected` is false.
        /// </summary>
        Detected = 0x181,

        /// <summary>
        /// Constant Variant (uint8_t). Determines which magnetic poles the sensor can detected,
        /// and whether or not it can measure their strength or just presence.
        ///
        /// ```
        /// const [variant] = jdunpack<[MagneticFieldLevelVariant]>(buf, "u8")
        /// ```
        /// </summary>
        Variant = 0x107,
    }

    public static class MagneticFieldLevelRegPack {
        /// <summary>
        /// Pack format for 'strength' register data.
        /// </summary>
        public const string Strength = "i1.15";

        /// <summary>
        /// Pack format for 'detected' register data.
        /// </summary>
        public const string Detected = "u8";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u8";
    }

    public enum MagneticFieldLevelEvent : ushort {
        /// <summary>
        /// Emitted when strong-enough magnetic field is detected.
        /// </summary>
        Active = 0x1,

        /// <summary>
        /// Emitted when strong-enough magnetic field is no longer detected.
        /// </summary>
        Inactive = 0x2,
    }

}
