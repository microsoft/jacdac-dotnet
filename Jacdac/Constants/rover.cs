namespace Jacdac {
    // Service: Rover
    public static class RoverConstants
    {
        public const uint ServiceClass = 0x19f4d06b;
    }
    public enum RoverReg {
        /**
         * The current position and orientation of the robot.
         *
         * ```
         * const [x, y, vx, vy, heading] = jdunpack<[number, number, number, number, number]>(buf, "i16.16 i16.16 i16.16 i16.16 i16.16")
         * ```
         */
        Kinematics = 0x101,
    }

    public static class RoverRegPack {
        /**
         * Pack format for 'kinematics' register data.
         */
        public const string Kinematics = "i16.16 i16.16 i16.16 i16.16 i16.16";
    }

}
