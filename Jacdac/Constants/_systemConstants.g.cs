namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint AnnounceInterval = 0x1f4;
    }

    public enum SystemReadingThreshold: byte { // uint8_t
        Neutral = 0x1,
        Inactive = 0x2,
        Active = 0x3,
    }


    public enum SystemStatusCodes: ushort { // uint16_t
        Ready = 0x0,
        Initializing = 0x1,
        Calibrating = 0x2,
        Sleeping = 0x3,
        WaitingForInput = 0x4,
        CalibrationNeeded = 0x64,
    }

    public enum SystemCmd {
        /// <summary>
        /// No args. Enumeration data for control service; service-specific advertisement data otherwise.
        /// Control broadcasts it automatically every ``announce_interval``ms, but other service have to be queried to provide it.
        /// </summary>
        Announce = 0x0,

        /// <summary>
        /// No args. Registers number `N` is fetched by issuing command `0x1000 | N`.
        /// The report format is the same as the format of the register.
        /// </summary>
        GetRegister = 0x1000,

        /// <summary>
        /// No args. Registers number `N` is set by issuing command `0x2000 | N`, with the format
        /// the same as the format of the register.
        /// </summary>
        SetRegister = 0x2000,

        /// <summary>
        /// Event from sensor or a broadcast service.
        ///
        /// ```
        /// const [eventId, eventArgument] = jdunpack<[number, number]>(buf, "u32 u32")
        /// ```
        /// </summary>
        Event = 0x1,

        /// <summary>
        /// No args. Request to calibrate a sensor. The report indicates the calibration is done.
        /// </summary>
        Calibrate = 0x2,

        /// <summary>
        /// This report may be emitted by a server in response to a command (action or register operation)
        /// that it does not understand.
        /// The `service_command` and `packet_crc` fields are copied from the command packet that was unhandled.
        /// Note that it's possible to get an ACK, followed by such an error report.
        ///
        /// ```
        /// const [serviceCommand, packetCrc] = jdunpack<[number, number]>(buf, "u16 u16")
        /// ```
        /// </summary>
        CommandNotImplemented = 0x3,
    }

    public static class SystemCmdPack {
        /// <summary>
        /// Pack format for 'event' register data.
        /// </summary>
        public const string Event = "u32 u32";

        /// <summary>
        /// Pack format for 'command_not_implemented' register data.
        /// </summary>
        public const string CommandNotImplemented = "u16 u16";
    }

    public enum SystemReg {
        /// <summary>
        /// Read-write uint32_t. This is either binary on/off (0 or non-zero), or can be gradual (eg. brightness of an RGB LED strip).
        ///
        /// ```
        /// const [intensity] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        Intensity = 0x1,

        /// <summary>
        /// Read-write int32_t. The primary value of actuator (eg. servo pulse length, or motor duty cycle).
        ///
        /// ```
        /// const [value] = jdunpack<[number]>(buf, "i32")
        /// ```
        /// </summary>
        Value = 0x2,

        /// <summary>
        /// Constant int32_t. The lowest value that can be reported for the value register.
        ///
        /// ```
        /// const [minValue] = jdunpack<[number]>(buf, "i32")
        /// ```
        /// </summary>
        MinValue = 0x110,

        /// <summary>
        /// Constant int32_t. The highest value that can be reported for the value register.
        ///
        /// ```
        /// const [maxValue] = jdunpack<[number]>(buf, "i32")
        /// ```
        /// </summary>
        MaxValue = 0x111,

        /// <summary>
        /// Read-write mA uint16_t. Limit the power drawn by the service, in mA.
        ///
        /// ```
        /// const [maxPower] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        MaxPower = 0x7,

        /// <summary>
        /// Read-write # uint8_t. Asks device to stream a given number of samples
        /// (clients will typically write `255` to this register every second or so, while streaming is required).
        ///
        /// ```
        /// const [streamingSamples] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        StreamingSamples = 0x3,

        /// <summary>
        /// Read-write ms uint32_t. Period between packets of data when streaming in milliseconds.
        ///
        /// ```
        /// const [streamingInterval] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        StreamingInterval = 0x4,

        /// <summary>
        /// Read-only int32_t. Read-only value of the sensor, also reported in streaming.
        ///
        /// ```
        /// const [reading] = jdunpack<[number]>(buf, "i32")
        /// ```
        /// </summary>
        Reading = 0x101,

        /// <summary>
        /// Read-write uint32_t. For sensors that support it, sets the range (sometimes also described `min`/`max_reading`).
        /// Typically only a small set of values is supported.
        /// Setting it to `X` will select the smallest possible range that is at least `X`,
        /// or if it doesn't exist, the largest supported range.
        ///
        /// ```
        /// const [readingRange] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        ReadingRange = 0x8,

        /// <summary>
        /// Constant. Lists the values supported as `reading_range`.
        ///
        /// ```
        /// const [range] = jdunpack<[number[]]>(buf, "u32[]")
        /// ```
        /// </summary>
        SupportedRanges = 0x10a,

        /// <summary>
        /// Constant int32_t. The lowest value that can be reported by the sensor.
        ///
        /// ```
        /// const [minReading] = jdunpack<[number]>(buf, "i32")
        /// ```
        /// </summary>
        MinReading = 0x104,

        /// <summary>
        /// Constant int32_t. The highest value that can be reported by the sensor.
        ///
        /// ```
        /// const [maxReading] = jdunpack<[number]>(buf, "i32")
        /// ```
        /// </summary>
        MaxReading = 0x105,

        /// <summary>
        /// Read-only uint32_t. The real value of whatever is measured is between `reading - reading_error` and `reading + reading_error`. It should be computed from the internal state of the sensor. This register is often, but not always `const`. If the register value is modified,
        /// send a report in the same frame of the ``reading`` report.
        ///
        /// ```
        /// const [readingError] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        ReadingError = 0x106,

        /// <summary>
        /// Constant uint32_t. Smallest, yet distinguishable change in reading.
        ///
        /// ```
        /// const [readingResolution] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        ReadingResolution = 0x108,

        /// <summary>
        /// Read-write int32_t. Threshold when reading data gets inactive and triggers a ``inactive``.
        ///
        /// ```
        /// const [inactiveThreshold] = jdunpack<[number]>(buf, "i32")
        /// ```
        /// </summary>
        InactiveThreshold = 0x5,

        /// <summary>
        /// Read-write int32_t. Thresholds when reading data gets active and triggers a ``active`` event.
        ///
        /// ```
        /// const [activeThreshold] = jdunpack<[number]>(buf, "i32")
        /// ```
        /// </summary>
        ActiveThreshold = 0x6,

        /// <summary>
        /// Constant ms uint32_t. Preferred default streaming interval for sensor in milliseconds.
        ///
        /// ```
        /// const [streamingPreferredInterval] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        StreamingPreferredInterval = 0x102,

        /// <summary>
        /// Constant uint32_t. The hardware variant of the service.
        /// For services which support this, there's an enum defining the meaning.
        ///
        /// ```
        /// const [variant] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        Variant = 0x107,

        /// <summary>
        /// Reports the current state or error status of the device. ``code`` is a standardized value from
        /// the Jacdac status/error codes. ``vendor_code`` is any vendor specific error code describing the device
        /// state. This report is typically not queried, when a device has an error, it will typically
        /// add this report in frame along with the announce packet.
        ///
        /// ```
        /// const [code, vendorCode] = jdunpack<[SystemStatusCodes, number]>(buf, "u16 u16")
        /// ```
        /// </summary>
        StatusCode = 0x103,

        /// <summary>
        /// Constant string (bytes). A friendly name that describes the role of this service instance in the device.
        ///
        /// ```
        /// const [instanceName] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        InstanceName = 0x109,
    }

    public static class SystemRegPack {
        /// <summary>
        /// Pack format for 'intensity' register data.
        /// </summary>
        public const string Intensity = "u32";

        /// <summary>
        /// Pack format for 'value' register data.
        /// </summary>
        public const string Value = "i32";

        /// <summary>
        /// Pack format for 'min_value' register data.
        /// </summary>
        public const string MinValue = "i32";

        /// <summary>
        /// Pack format for 'max_value' register data.
        /// </summary>
        public const string MaxValue = "i32";

        /// <summary>
        /// Pack format for 'max_power' register data.
        /// </summary>
        public const string MaxPower = "u16";

        /// <summary>
        /// Pack format for 'streaming_samples' register data.
        /// </summary>
        public const string StreamingSamples = "u8";

        /// <summary>
        /// Pack format for 'streaming_interval' register data.
        /// </summary>
        public const string StreamingInterval = "u32";

        /// <summary>
        /// Pack format for 'reading' register data.
        /// </summary>
        public const string Reading = "i32";

        /// <summary>
        /// Pack format for 'reading_range' register data.
        /// </summary>
        public const string ReadingRange = "u32";

        /// <summary>
        /// Pack format for 'supported_ranges' register data.
        /// </summary>
        public const string SupportedRanges = "r: u32";

        /// <summary>
        /// Pack format for 'min_reading' register data.
        /// </summary>
        public const string MinReading = "i32";

        /// <summary>
        /// Pack format for 'max_reading' register data.
        /// </summary>
        public const string MaxReading = "i32";

        /// <summary>
        /// Pack format for 'reading_error' register data.
        /// </summary>
        public const string ReadingError = "u32";

        /// <summary>
        /// Pack format for 'reading_resolution' register data.
        /// </summary>
        public const string ReadingResolution = "u32";

        /// <summary>
        /// Pack format for 'inactive_threshold' register data.
        /// </summary>
        public const string InactiveThreshold = "i32";

        /// <summary>
        /// Pack format for 'active_threshold' register data.
        /// </summary>
        public const string ActiveThreshold = "i32";

        /// <summary>
        /// Pack format for 'streaming_preferred_interval' register data.
        /// </summary>
        public const string StreamingPreferredInterval = "u32";

        /// <summary>
        /// Pack format for 'variant' register data.
        /// </summary>
        public const string Variant = "u32";

        /// <summary>
        /// Pack format for 'status_code' register data.
        /// </summary>
        public const string StatusCode = "u16 u16";

        /// <summary>
        /// Pack format for 'instance_name' register data.
        /// </summary>
        public const string InstanceName = "s";
    }

    public enum SystemEvent {
        /// <summary>
        /// Notifies that the service has been activated (eg. button pressed, network connected, etc.)
        /// </summary>
        Active = 0x1,

        /// <summary>
        /// Notifies that the service has been dis-activated.
        /// </summary>
        Inactive = 0x2,

        /// <summary>
        /// Notifies that the some state of the service changed.
        /// </summary>
        Change = 0x3,

        /// <summary>
        /// Notifies that the status code of the service changed.
        ///
        /// ```
        /// const [code, vendorCode] = jdunpack<[SystemStatusCodes, number]>(buf, "u16 u16")
        /// ```
        /// </summary>
        StatusCodeChanged = 0x4,

        /// <summary>
        /// Notifies that the threshold is back between ``low`` and ``high``.
        /// </summary>
        Neutral = 0x7,
    }

    public static class SystemEventPack {
        /// <summary>
        /// Pack format for 'status_code_changed' register data.
        /// </summary>
        public const string StatusCodeChanged = "u16 u16";
    }

}
