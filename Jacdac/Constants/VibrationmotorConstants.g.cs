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

}
