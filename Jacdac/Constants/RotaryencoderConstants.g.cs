namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint RotaryEncoder = 0x10fa29c9;
    }
    public enum RotaryEncoderReg : ushort {
        /// <summary>
        /// Read-only # int32_t. Upon device reset starts at `0` (regardless of the shaft position).
        /// Increases by `1` for a clockwise "click", by `-1` for counter-clockwise.
        ///
        /// ```
        /// const [position] = jdunpack<[number]>(buf, "i32")
        /// ```
        /// </summary>
        Position = 0x101,

        /// <summary>
        /// Constant # uint16_t. This specifies by how much `position` changes when the crank does 360 degree turn. Typically 12 or 24.
        ///
        /// ```
        /// const [clicksPerTurn] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        ClicksPerTurn = 0x180,

        /// <summary>
        /// Constant bool (uint8_t). The encoder is combined with a clicker. If this is the case, the clicker button service
        /// should follow this service in the service list of the device.
        ///
        /// ```
        /// const [clicker] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Clicker = 0x181,
    }

    public static class RotaryEncoderRegPack {
        /// <summary>
        /// Pack format for 'position' register data.
        /// </summary>
        public const string Position = "i32";

        /// <summary>
        /// Pack format for 'clicks_per_turn' register data.
        /// </summary>
        public const string ClicksPerTurn = "u16";

        /// <summary>
        /// Pack format for 'clicker' register data.
        /// </summary>
        public const string Clicker = "u8";
    }

}
