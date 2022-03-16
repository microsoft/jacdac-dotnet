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
        /// Read-write mV i22.10 (int32_t). The current output voltage of the power supply. Values provided must be in the range `minimum_voltage` to `maximum_voltage`
        ///
        /// ```
        /// const [outputVoltage] = jdunpack<[number]>(buf, "i22.10")
        /// ```
        /// </summary>
        OutputVoltage = 0x2,

        /// <summary>
        /// Constant mV i22.10 (int32_t). The minimum output voltage of the power supply. For fixed power supplies, `minimum_voltage` should be equal to `maximum_voltage`.
        ///
        /// ```
        /// const [minimumVoltage] = jdunpack<[number]>(buf, "i22.10")
        /// ```
        /// </summary>
        MinimumVoltage = 0x110,

        /// <summary>
        /// Constant mV i22.10 (int32_t). The maximum output voltage of the power supply. For fixed power supplies, `minimum_voltage` should be equal to `maximum_voltage`.
        ///
        /// ```
        /// const [maximumVoltage] = jdunpack<[number]>(buf, "i22.10")
        /// ```
        /// </summary>
        MaximumVoltage = 0x111,
    }

    public static class PowerSupplyRegPack {
        /// <summary>
        /// Pack format for 'enabled' register data.
        /// </summary>
        public const string Enabled = "u8";

        /// <summary>
        /// Pack format for 'output_voltage' register data.
        /// </summary>
        public const string OutputVoltage = "i22.10";

        /// <summary>
        /// Pack format for 'minimum_voltage' register data.
        /// </summary>
        public const string MinimumVoltage = "i22.10";

        /// <summary>
        /// Pack format for 'maximum_voltage' register data.
        /// </summary>
        public const string MaximumVoltage = "i22.10";
    }

}
