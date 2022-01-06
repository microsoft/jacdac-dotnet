namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Servo = 0x12fc9103;
    }
    public enum ServoReg {
        /// <summary>
        /// Read-write ° i16.16 (int32_t). Specifies the angle of the arm (request).
        ///
        /// ```
        /// const [angle] = jdunpack<[number]>(buf, "i16.16")
        /// ```
        /// </summary>
        Angle = 0x2,

        /// <summary>
        /// Read-write bool (uint8_t). Turn the power to the servo on/off.
        ///
        /// ```
        /// const [enabled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Enabled = 0x1,

        /// <summary>
        /// Read-write ° i16.16 (int32_t). Correction applied to the angle to account for the servo arm drift.
        ///
        /// ```
        /// const [offset] = jdunpack<[number]>(buf, "i16.16")
        /// ```
        /// </summary>
        Offset = 0x81,

        /// <summary>
        /// Constant ° i16.16 (int32_t). Lowest angle that can be set.
        ///
        /// ```
        /// const [minAngle] = jdunpack<[number]>(buf, "i16.16")
        /// ```
        /// </summary>
        MinAngle = 0x110,

        /// <summary>
        /// Read-write μs uint16_t. The length of pulse corresponding to lowest angle.
        ///
        /// ```
        /// const [minPulse] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        MinPulse = 0x83,

        /// <summary>
        /// Constant ° i16.16 (int32_t). Highest angle that can be set.
        ///
        /// ```
        /// const [maxAngle] = jdunpack<[number]>(buf, "i16.16")
        /// ```
        /// </summary>
        MaxAngle = 0x111,

        /// <summary>
        /// Read-write μs uint16_t. The length of pulse corresponding to highest angle.
        ///
        /// ```
        /// const [maxPulse] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        MaxPulse = 0x85,

        /// <summary>
        /// Constant kg/cm u16.16 (uint32_t). The servo motor will stop rotating when it is trying to move a ``stall_torque`` weight at a radial distance of ``1.0`` cm.
        ///
        /// ```
        /// const [stallTorque] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        StallTorque = 0x180,

        /// <summary>
        /// Constant s/60° u16.16 (uint32_t). Time to move 60°.
        ///
        /// ```
        /// const [responseSpeed] = jdunpack<[number]>(buf, "u16.16")
        /// ```
        /// </summary>
        ResponseSpeed = 0x181,

        /// <summary>
        /// Read-only ° i16.16 (int32_t). The current physical position of the arm.
        ///
        /// ```
        /// const [currentAngle] = jdunpack<[number]>(buf, "i16.16")
        /// ```
        /// </summary>
        CurrentAngle = 0x101,
    }

    public static class ServoRegPack {
        /// <summary>
        /// Pack format for 'angle' register data.
        /// </summary>
        public const string Angle = "i16.16";

        /// <summary>
        /// Pack format for 'enabled' register data.
        /// </summary>
        public const string Enabled = "u8";

        /// <summary>
        /// Pack format for 'offset' register data.
        /// </summary>
        public const string Offset = "i16.16";

        /// <summary>
        /// Pack format for 'min_angle' register data.
        /// </summary>
        public const string MinAngle = "i16.16";

        /// <summary>
        /// Pack format for 'min_pulse' register data.
        /// </summary>
        public const string MinPulse = "u16";

        /// <summary>
        /// Pack format for 'max_angle' register data.
        /// </summary>
        public const string MaxAngle = "i16.16";

        /// <summary>
        /// Pack format for 'max_pulse' register data.
        /// </summary>
        public const string MaxPulse = "u16";

        /// <summary>
        /// Pack format for 'stall_torque' register data.
        /// </summary>
        public const string StallTorque = "u16.16";

        /// <summary>
        /// Pack format for 'response_speed' register data.
        /// </summary>
        public const string ResponseSpeed = "u16.16";

        /// <summary>
        /// Pack format for 'current_angle' register data.
        /// </summary>
        public const string CurrentAngle = "i16.16";
    }

}
