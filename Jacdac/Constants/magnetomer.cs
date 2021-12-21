namespace Jacdac {
    // Service: Magnetometer
    public static class MagnetometerConstants
    {
        public const uint ServiceClass = 0x13029088;
    }
    public enum MagnetometerReg {
        /**
         * Indicates the current magnetic field on magnetometer.
         * For reference: `1 mgauss` is `100 nT` (and `1 gauss` is `100 000 nT`).
         *
         * ```
         * const [x, y, z] = jdunpack<[number, number, number]>(buf, "i32 i32 i32")
         * ```
         */
        Forces = 0x101,

        /**
         * Read-only nT int32_t. Absolute estimated error on the readings.
         *
         * ```
         * const [forcesError] = jdunpack<[number]>(buf, "i32")
         * ```
         */
        ForcesError = 0x106,
    }

    public static class MagnetometerRegPack {
        /**
         * Pack format for 'forces' register data.
         */
        public const string Forces = "i32 i32 i32";

        /**
         * Pack format for 'forces_error' register data.
         */
        public const string ForcesError = "i32";
    }

    public enum MagnetometerCmd {
        /**
         * No args. Forces a calibration sequence where the user/device
         * might have to rotate to be calibrated.
         */
        Calibrate = 0x2,
    }

}
