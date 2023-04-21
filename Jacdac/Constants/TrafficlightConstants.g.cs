namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint TrafficLight = 0x15c38d9b;
    }
    public enum TrafficLightReg : ushort {
        /// <summary>
        /// Read-write bool (uint8_t). The on/off state of the red light.
        ///
        /// ```
        /// const [red] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Red = 0x80,

        /// <summary>
        /// Read-write bool (uint8_t). The on/off state of the yellow light.
        ///
        /// ```
        /// const [yellow] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Yellow = 0x81,

        /// <summary>
        /// Read-write bool (uint8_t). The on/off state of the green light.
        ///
        /// ```
        /// const [green] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Green = 0x82,
    }

    public static class TrafficLightRegPack {
        /// <summary>
        /// Pack format for 'red' data.
        /// </summary>
        public const string Red = "u8";

        /// <summary>
        /// Pack format for 'yellow' data.
        /// </summary>
        public const string Yellow = "u8";

        /// <summary>
        /// Pack format for 'green' data.
        /// </summary>
        public const string Green = "u8";
    }

}
