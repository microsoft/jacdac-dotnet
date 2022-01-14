namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint PressureButton = 0x281740c3;
    }
    public enum PressureButtonReg : ushort {
        /// <summary>
        /// Read-write ratio u0.16 (uint16_t). Indicates the threshold for ``up`` events.
        ///
        /// ```
        /// const [threshold] = jdunpack<[number]>(buf, "u0.16")
        /// ```
        /// </summary>
        Threshold = 0x6,
    }

    public static class PressureButtonRegPack {
        /// <summary>
        /// Pack format for 'threshold' register data.
        /// </summary>
        public const string Threshold = "u0.16";
    }

}
