namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Accelerometer = 0x1f140409;
    }
    public enum AccelerometerReg : ushort {
        /// <summary>
        /// Indicates the current forces acting on accelerometer.
        ///
        /// ```
        /// const [x, y, z] = jdunpack<[number, number, number]>(buf, "i12.20 i12.20 i12.20")
        /// ```
        /// </summary>
        Forces = 0x101,

        /// <summary>
        /// Read-only g u12.20 (uint32_t). Error on the reading value.
        ///
        /// ```
        /// const [forcesError] = jdunpack<[number]>(buf, "u12.20")
        /// ```
        /// </summary>
        ForcesError = 0x106,

        /// <summary>
        /// Read-write g u12.20 (uint32_t). Configures the range forces detected.
        /// The value will be "rounded up" to one of `max_forces_supported`.
        ///
        /// ```
        /// const [maxForce] = jdunpack<[number]>(buf, "u12.20")
        /// ```
        /// </summary>
        MaxForce = 0x8,

        /// <summary>
        /// Constant. Lists values supported for writing `max_force`.
        ///
        /// ```
        /// const [maxForce] = jdunpack<[number[]]>(buf, "u12.20[]")
        /// ```
        /// </summary>
        MaxForcesSupported = 0x10a,
    }

    public static class AccelerometerRegPack {
        /// <summary>
        /// Pack format for 'forces' data.
        /// </summary>
        public const string Forces = "i12.20 i12.20 i12.20";

        /// <summary>
        /// Pack format for 'forces_error' data.
        /// </summary>
        public const string ForcesError = "u12.20";

        /// <summary>
        /// Pack format for 'max_force' data.
        /// </summary>
        public const string MaxForce = "u12.20";

        /// <summary>
        /// Pack format for 'max_forces_supported' data.
        /// </summary>
        public const string MaxForcesSupported = "r: u12.20";
    }

    public enum AccelerometerEvent : ushort {
        /// <summary>
        /// Emitted when accelerometer is tilted in the given direction.
        /// </summary>
        TiltUp = 0x81,

        /// <summary>
        /// Emitted when accelerometer is tilted in the given direction.
        /// </summary>
        TiltDown = 0x82,

        /// <summary>
        /// Emitted when accelerometer is tilted in the given direction.
        /// </summary>
        TiltLeft = 0x83,

        /// <summary>
        /// Emitted when accelerometer is tilted in the given direction.
        /// </summary>
        TiltRight = 0x84,

        /// <summary>
        /// Emitted when accelerometer is laying flat in the given direction.
        /// </summary>
        FaceUp = 0x85,

        /// <summary>
        /// Emitted when accelerometer is laying flat in the given direction.
        /// </summary>
        FaceDown = 0x86,

        /// <summary>
        /// Emitted when total force acting on accelerometer is much less than 1g.
        /// </summary>
        Freefall = 0x87,

        /// <summary>
        /// Emitted when forces change violently a few times.
        /// </summary>
        Shake = 0x8b,

        /// <summary>
        /// Emitted when force in any direction exceeds given threshold.
        /// </summary>
        Force2g = 0x8c,

        /// <summary>
        /// Emitted when force in any direction exceeds given threshold.
        /// </summary>
        Force3g = 0x88,

        /// <summary>
        /// Emitted when force in any direction exceeds given threshold.
        /// </summary>
        Force6g = 0x89,

        /// <summary>
        /// Emitted when force in any direction exceeds given threshold.
        /// </summary>
        Force8g = 0x8a,
    }

}
