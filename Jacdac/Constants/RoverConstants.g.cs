namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Rover = 0x19f4d06b;
    }
    public enum RoverReg : ushort {
        /// <summary>
        /// The current position and orientation of the robot.
        ///
        /// ```
        /// const [x, y, vx, vy, heading] = jdunpack<[number, number, number, number, number]>(buf, "i16.16 i16.16 i16.16 i16.16 i16.16")
        /// ```
        /// </summary>
        Kinematics = 0x101,
    }

    public static class RoverRegPack {
        /// <summary>
        /// Pack format for 'kinematics' data.
        /// </summary>
        public const string Kinematics = "i16.16 i16.16 i16.16 i16.16 i16.16";
    }

}
