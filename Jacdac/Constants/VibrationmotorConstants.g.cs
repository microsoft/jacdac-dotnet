namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint VibrationMotor = 0x183fc4a2;
    }
    public enum VibrationMotorCmd : ushort {
        /// <summary>
        /// Starts a sequence of vibration and pauses. To stop any existing vibration, send an empty payload.
        ///
        /// ```
        /// const [rest] = jdunpack<[([number, number])[]]>(buf, "r: u8 u0.8")
        /// const [duration, intensity] = rest[0]
        /// ```
        /// </summary>
        Vibrate = 0x80,
    }

    public static class VibrationMotorCmdPack {
        /// <summary>
        /// Pack format for 'vibrate' register data.
        /// </summary>
        public const string Vibrate = "r: u8 u0.8";
    }

    public enum VibrationMotorReg : ushort {
        /// <summary>
        /// Constant uint8_t. The maximum number of vibration sequences supported in a single packet.
        ///
        /// ```
        /// const [maxVibrations] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        MaxVibrations = 0x180,
    }

    public static class VibrationMotorRegPack {
        /// <summary>
        /// Pack format for 'max_vibrations' register data.
        /// </summary>
        public const string MaxVibrations = "u8";
    }

}
