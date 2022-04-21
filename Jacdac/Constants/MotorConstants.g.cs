namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Motor = 0x17004cd8;
    }
    public enum MotorReg : ushort {
        /// <summary>
        /// Read-write ratio i1.15 (int16_t). Relative speed of the motor. Use positive/negative values to run the motor forwards and backwards.
        /// Positive is recommended to be clockwise rotation and negative counterclockwise. A speed of ``0``
        /// while ``enabled`` acts as brake.
        ///
        /// ```
        /// const [speed] = jdunpack<[number]>(buf, "i1.15")
        /// ```
        /// </summary>
        Speed = 0x2,

        /// <summary>
        /// Read-write bool (uint8_t). Turn the power to the motor on/off.
        ///
        /// ```
        /// const [enabled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Enabled = 0x1,

        /// <summary>
        /// Constant kg/cm u16.16 (uint32_t). Torque required to produce the rated power of an electrical motor at load speed.
        ///
        /// ```
        /// const [loadTorque] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        LoadTorque = 0x180,

        /// <summary>
        /// Constant rpm u16.16 (uint32_t). Revolutions per minute of the motor under full load.
        ///
        /// ```
        /// const [loadRotationSpeed] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        LoadRotationSpeed = 0x181,

        /// <summary>
        /// Constant bool (uint8_t). Indicates if the motor can run backwards.
        ///
        /// ```
        /// const [reversible] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Reversible = 0x182,
    }

    public static class MotorRegPack {
        /// <summary>
        /// Pack format for 'speed' register data.
        /// </summary>
        public const string Speed = "i1.15";

        /// <summary>
        /// Pack format for 'enabled' register data.
        /// </summary>
        public const string Enabled = "u8";

        /// <summary>
        /// Pack format for 'load_torque' register data.
        /// </summary>
        public const string LoadTorque = "u16.16";

        /// <summary>
        /// Pack format for 'load_rotation_speed' register data.
        /// </summary>
        public const string LoadRotationSpeed = "u16.16";

        /// <summary>
        /// Pack format for 'reversible' register data.
        /// </summary>
        public const string Reversible = "u8";
    }

}
