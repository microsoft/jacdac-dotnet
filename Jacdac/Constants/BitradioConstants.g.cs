namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint BitRadio = 0x1ac986cf;
    }
    public enum BitRadioReg : ushort {
        /// <summary>
        /// Read-write bool (uint8_t). Turns on/off the radio antenna.
        ///
        /// ```
        /// const [enabled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Enabled = 0x1,

        /// <summary>
        /// Read-write uint8_t. Group used to filter packets
        ///
        /// ```
        /// const [group] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Group = 0x80,

        /// <summary>
        /// Read-write uint8_t. Antenna power to increase or decrease range.
        ///
        /// ```
        /// const [transmissionPower] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        TransmissionPower = 0x81,

        /// <summary>
        /// Read-write uint8_t. Change the transmission and reception band of the radio to the given channel.
        ///
        /// ```
        /// const [frequencyBand] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        FrequencyBand = 0x82,
    }

    public static class BitRadioRegPack {
        /// <summary>
        /// Pack format for 'enabled' register data.
        /// </summary>
        public const string Enabled = "u8";

        /// <summary>
        /// Pack format for 'group' register data.
        /// </summary>
        public const string Group = "u8";

        /// <summary>
        /// Pack format for 'transmission_power' register data.
        /// </summary>
        public const string TransmissionPower = "u8";

        /// <summary>
        /// Pack format for 'frequency_band' register data.
        /// </summary>
        public const string FrequencyBand = "u8";
    }

    public enum BitRadioCmd : ushort {
        /// <summary>
        /// Argument: message string (bytes). Sends a string payload as a radio message, maximum 18 characters.
        ///
        /// ```
        /// const [message] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        SendString = 0x80,

        /// <summary>
        /// Argument: value f64 (uint64_t). Sends a double precision number payload as a radio message
        ///
        /// ```
        /// const [value] = jdunpack<[number]>(buf, "f64")
        /// ```
        /// </summary>
        SendNumber = 0x81,

        /// <summary>
        /// Sends a double precision number and a name payload as a radio message
        ///
        /// ```
        /// const [value, name] = jdunpack<[number, string]>(buf, "f64 s")
        /// ```
        /// </summary>
        SendValue = 0x82,

        /// <summary>
        /// Argument: data bytes. Sends a payload of bytes as a radio message
        ///
        /// ```
        /// const [data] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        SendBuffer = 0x83,

        /// <summary>
        /// Raised when a string packet is received
        ///
        /// ```
        /// const [time, deviceSerialNumber, rssi, message] = jdunpack<[number, number, number, string]>(buf, "u32 u32 i8 x[1] s")
        /// ```
        /// </summary>
        StringReceived = 0x90,

        /// <summary>
        /// Raised when a number packet is received
        ///
        /// ```
        /// const [time, deviceSerialNumber, rssi, value, name] = jdunpack<[number, number, number, number, string]>(buf, "u32 u32 i8 x[3] f64 s")
        /// ```
        /// </summary>
        NumberReceived = 0x91,

        /// <summary>
        /// Raised when a buffer packet is received
        ///
        /// ```
        /// const [time, deviceSerialNumber, rssi, data] = jdunpack<[number, number, number, Uint8Array]>(buf, "u32 u32 i8 x[1] b")
        /// ```
        /// </summary>
        BufferReceived = 0x92,
    }

    public static class BitRadioCmdPack {
        /// <summary>
        /// Pack format for 'send_string' register data.
        /// </summary>
        public const string SendString = "s";

        /// <summary>
        /// Pack format for 'send_number' register data.
        /// </summary>
        public const string SendNumber = "f64";

        /// <summary>
        /// Pack format for 'send_value' register data.
        /// </summary>
        public const string SendValue = "f64 s";

        /// <summary>
        /// Pack format for 'send_buffer' register data.
        /// </summary>
        public const string SendBuffer = "b";

        /// <summary>
        /// Pack format for 'string_received' register data.
        /// </summary>
        public const string StringReceived = "u32 u32 i8 b[1] s";

        /// <summary>
        /// Pack format for 'number_received' register data.
        /// </summary>
        public const string NumberReceived = "u32 u32 i8 b[3] f64 s";

        /// <summary>
        /// Pack format for 'buffer_received' register data.
        /// </summary>
        public const string BufferReceived = "u32 u32 i8 b[1] b";
    }

}
