namespace Jacdac {
    // Service: bit:radio
    public static class BitRadioConstants
    {
        public const uint ServiceClass = 0x1ac986cf;
    }
    public enum BitRadioReg {
        /**
         * Read-write bool (uint8_t). Turns on/off the radio antenna.
         *
         * ```
         * const [enabled] = jdunpack<[number]>(buf, "u8")
         * ```
         */
        Enabled = 0x1,

        /**
         * Read-write uint8_t. Group used to filter packets
         *
         * ```
         * const [group] = jdunpack<[number]>(buf, "u8")
         * ```
         */
        Group = 0x80,

        /**
         * Read-write uint8_t. Antenna power to increase or decrease range.
         *
         * ```
         * const [transmissionPower] = jdunpack<[number]>(buf, "u8")
         * ```
         */
        TransmissionPower = 0x81,

        /**
         * Read-write uint8_t. Change the transmission and reception band of the radio to the given channel.
         *
         * ```
         * const [frequencyBand] = jdunpack<[number]>(buf, "u8")
         * ```
         */
        FrequencyBand = 0x82,
    }

    public static class BitRadioRegPack {
        /**
         * Pack format for 'enabled' register data.
         */
        public const string Enabled = "u8";

        /**
         * Pack format for 'group' register data.
         */
        public const string Group = "u8";

        /**
         * Pack format for 'transmission_power' register data.
         */
        public const string TransmissionPower = "u8";

        /**
         * Pack format for 'frequency_band' register data.
         */
        public const string FrequencyBand = "u8";
    }

    public enum BitRadioCmd {
        /**
         * Argument: message string (bytes). Sends a string payload as a radio message, maximum 18 characters.
         *
         * ```
         * const [message] = jdunpack<[string]>(buf, "s")
         * ```
         */
        SendString = 0x80,

        /**
         * Argument: value f64 (uint64_t). Sends a double precision number payload as a radio message
         *
         * ```
         * const [value] = jdunpack<[number]>(buf, "f64")
         * ```
         */
        SendNumber = 0x81,

        /**
         * Sends a double precision number and a name payload as a radio message
         *
         * ```
         * const [value, name] = jdunpack<[number, string]>(buf, "f64 s")
         * ```
         */
        SendValue = 0x82,

        /**
         * Argument: data bytes. Sends a payload of bytes as a radio message
         *
         * ```
         * const [data] = jdunpack<[Uint8Array]>(buf, "b")
         * ```
         */
        SendBuffer = 0x83,

        /**
         * Raised when a string packet is received
         *
         * ```
         * const [time, deviceSerialNumber, rssi, message] = jdunpack<[number, number, number, string]>(buf, "u32 u32 i8 x[1] s")
         * ```
         */
        StringReceived = 0x90,

        /**
         * Raised when a number packet is received
         *
         * ```
         * const [time, deviceSerialNumber, rssi, value, name] = jdunpack<[number, number, number, number, string]>(buf, "u32 u32 i8 x[3] f64 s")
         * ```
         */
        NumberReceived = 0x91,

        /**
         * Raised when a buffer packet is received
         *
         * ```
         * const [time, deviceSerialNumber, rssi, data] = jdunpack<[number, number, number, Uint8Array]>(buf, "u32 u32 i8 x[1] b")
         * ```
         */
        BufferReceived = 0x92,
    }

    public static class BitRadioCmdPack {
        /**
         * Pack format for 'send_string' register data.
         */
        public const string SendString = "s";

        /**
         * Pack format for 'send_number' register data.
         */
        public const string SendNumber = "f64";

        /**
         * Pack format for 'send_value' register data.
         */
        public const string SendValue = "f64 s";

        /**
         * Pack format for 'send_buffer' register data.
         */
        public const string SendBuffer = "b";

        /**
         * Pack format for 'string_received' register data.
         */
        public const string StringReceived = "u32 u32 i8 b[1] s";

        /**
         * Pack format for 'number_received' register data.
         */
        public const string NumberReceived = "u32 u32 i8 b[3] f64 s";

        /**
         * Pack format for 'buffer_received' register data.
         */
        public const string BufferReceived = "u32 u32 i8 b[1] b";
    }

}
