namespace Jacdac {
    // Service: Vibration motor
    public static class VibrationMotorConstants
    {
        public const uint ServiceClass = 0x183fc4a2;
    }
    public enum VibrationMotorCmd {
        /**
         * Starts a sequence of vibration and pauses. To stop any existing vibration, send an empty payload.
         *
         * ```
         * const [rest] = jdunpack<[([number, number])[]]>(buf, "r: u8 u0.8")
         * const [duration, intensity] = rest[0]
         * ```
         */
        Vibrate = 0x80,
    }

}
