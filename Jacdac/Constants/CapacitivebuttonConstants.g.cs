namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint CapacitiveButton = 0x2865adc9;
    }
    public enum CapacitiveButtonReg {
        /// <summary>
        /// Read-write ratio u0.16 (uint16_t). Indicates the threshold for ``up`` events.
        ///
        /// ```
        /// const [threshold] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        Threshold = 0x6,
    }

    public static class CapacitiveButtonRegPack {
        /// <summary>
        /// Pack format for 'threshold' register data.
        /// </summary>
        public const string Threshold = "u0.16";
    }

    public enum CapacitiveButtonCmd {
        /// <summary>
        /// No args. Request to calibrate the capactive. When calibration is requested, the device expects that no object is touching the button.
        /// The report indicates the calibration is done.
        /// </summary>
        Calibrate = 0x2,
    }

}
