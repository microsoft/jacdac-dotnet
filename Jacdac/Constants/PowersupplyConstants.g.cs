namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint PowerSupply = 0x1f40375f;
    }
    public enum PowerSupplyReg : ushort {
        /// <summary>
        /// Read-write bool (uint8_t). Turns the power supply on with `true`, off with `false`.
        ///
        /// ```
        /// const [enabled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Enabled = 0x1,

        /// <summary>
        /// Read-write V f64 (uint64_t). The current output voltage of the power supply. Values provided must be in the range `minimum_voltage` to `maximum_voltage`
        ///
        /// ```
        /// const [outputVoltage] = jdunpack<[number]>(buf, "f64")
        /// ```
        /// </summary>
        OutputVoltage = 0x2,

        /// <summary>
        /// Constant V f64 (uint64_t). The minimum output voltage of the power supply. For fixed power supplies, `minimum_voltage` should be equal to `maximum_voltage`.
        ///
        /// ```
        /// const [minimumVoltage] = jdunpack<[number]>(buf, "f64")
        /// ```
        /// </summary>
        MinimumVoltage = 0x110,

        /// <summary>
        /// Constant V f64 (uint64_t). The maximum output voltage of the power supply. For fixed power supplies, `minimum_voltage` should be equal to `maximum_voltage`.
        ///
        /// ```
        /// const [maximumVoltage] = jdunpack<[number]>(buf, "f64")
        /// ```
        /// </summary>
        MaximumVoltage = 0x111,
    }

    public static class PowerSupplyRegPack {
        /// <summary>
        /// Pack format for 'enabled' data.
        /// </summary>
        public const string Enabled = "u8";

        /// <summary>
        /// Pack format for 'output_voltage' data.
        /// </summary>
        public const string OutputVoltage = "f64";

        /// <summary>
        /// Pack format for 'minimum_voltage' data.
        /// </summary>
        public const string MinimumVoltage = "f64";

        /// <summary>
        /// Pack format for 'maximum_voltage' data.
        /// </summary>
        public const string MaximumVoltage = "f64";
    }

}
