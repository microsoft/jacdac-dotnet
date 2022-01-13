namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Illuminance = 0x1e6ecaf2;
    }
    public enum IlluminanceReg : ushort {
        /// <summary>
        /// Read-only lux u22.10 (uint32_t). The amount of illuminance, as lumens per square metre.
        ///
        /// ```
        /// const [illuminance] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        Illuminance = 0x101,

        /// <summary>
        /// Read-only lux u22.10 (uint32_t). Error on the reported sensor value.
        ///
        /// ```
        /// const [illuminanceError] = jdunpack<[number]>(buf, "u22.10")
        /// ```
        /// </summary>
        IlluminanceError = 0x106,
    }

    public static class IlluminanceRegPack {
        /// <summary>
        /// Pack format for 'illuminance' register data.
        /// </summary>
        public const string Illuminance = "u22.10";

        /// <summary>
        /// Pack format for 'illuminance_error' register data.
        /// </summary>
        public const string IlluminanceError = "u22.10";
    }

}
