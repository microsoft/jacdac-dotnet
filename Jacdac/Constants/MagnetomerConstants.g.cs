namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Magnetometer = 0x13029088;
    }
    public enum MagnetometerReg : ushort {
        /// <summary>
        /// Indicates the current magnetic field on magnetometer.
        /// For reference: `1 mgauss` is `100 nT` (and `1 gauss` is `100 000 nT`).
        ///
        /// ```
        /// const [x, y, z] = jdunpack<[number, number, number]>(buf, "i32 i32 i32")
        /// ```
        /// </summary>
        Forces = 0x101,

        /// <summary>
        /// Read-only nT int32_t. Absolute estimated error on the readings.
        ///
        /// ```
        /// const [forcesError] = jdunpack<[number]>(buf, "i32")
        /// ```
        /// </summary>
        ForcesError = 0x106,
    }

    public static class MagnetometerRegPack {
        /// <summary>
        /// Pack format for 'forces' data.
        /// </summary>
        public const string Forces = "i32 i32 i32";

        /// <summary>
        /// Pack format for 'forces_error' data.
        /// </summary>
        public const string ForcesError = "i32";
    }

    public enum MagnetometerCmd : ushort {
        /// <summary>
        /// No args. Forces a calibration sequence where the user/device
        /// might have to rotate to be calibrated.
        /// </summary>
        Calibrate = 0x2,
    }

}
