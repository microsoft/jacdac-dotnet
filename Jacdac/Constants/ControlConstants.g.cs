namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Control = 0x0;
    }

    [System.Flags]
    public enum ControlAnnounceFlags: ushort { // uint16_t
        RestartCounterSteady = 0xf,
        RestartCounter1 = 0x1,
        RestartCounter2 = 0x2,
        RestartCounter4 = 0x4,
        RestartCounter8 = 0x8,
        StatusLightNone = 0x0,
        StatusLightMono = 0x10,
        StatusLightRgbNoFade = 0x20,
        StatusLightRgbFade = 0x30,
        SupportsACK = 0x100,
        SupportsBroadcast = 0x200,
        SupportsFrames = 0x400,
        IsClient = 0x800,
        SupportsReliableCommands = 0x1000,
    }

    public enum ControlCmd : ushort {
        /// <summary>
        /// No args. The `restart_counter` is computed from the `flags & RestartCounterSteady`, starts at `0x1` and increments by one until it reaches `0xf`, then it stays at `0xf`.
        /// If this number ever goes down, it indicates that the device restarted.
        /// `service_class` indicates class identifier for each service index (service index `0` is always control, so it's
        /// skipped in this enumeration).
        /// `packet_count` indicates the number of reports sent by the current device since last announce,
        /// including the current announce packet (it is always 0 if this feature is not supported).
        /// The command form can be used to induce report, which is otherwise broadcast every 500ms.
        /// </summary>
        Services = 0x0,

        /// <summary>
        /// report Services
        /// ```
        /// const [flags, packetCount, serviceClass] = jdunpack<[ControlAnnounceFlags, number, number[]]>(buf, "u16 u8 x[1] u32[]")
        /// ```
        /// </summary>

        /// <summary>
        /// No args. Do nothing. Always ignored. Can be used to test ACKs.
        /// </summary>
        Noop = 0x80,

        /// <summary>
        /// No args. Blink the status LED (262ms on, 262ms off, four times, with the blue LED) or otherwise draw user's attention to device with no status light.
        /// For devices with status light (this can be discovered in the announce flags), the client should
        /// send the sequence of status light command to generate the identify animation.
        /// </summary>
        Identify = 0x81,

        /// <summary>
        /// No args. Reset device. ACK may or may not be sent.
        /// </summary>
        Reset = 0x82,

        /// <summary>
        /// The device will respond `num_responses` times, as fast as it can, setting the `counter` field in the report
        /// to `start_counter`, then `start_counter + 1`, ..., and finally `start_counter + num_responses - 1`.
        /// The `dummy_payload` is `size` bytes long and contains bytes `0, 1, 2, ...`.
        ///
        /// ```
        /// const [numResponses, startCounter, size] = jdunpack<[number, number, number]>(buf, "u32 u32 u8")
        /// ```
        /// </summary>
        FloodPing = 0x83,

        /// <summary>
        /// report FloodPing
        /// ```
        /// const [counter, dummyPayload] = jdunpack<[number, Uint8Array]>(buf, "u32 b")
        /// ```
        /// </summary>

        /// <summary>
        /// Initiates a color transition of the status light from its current color to the one specified.
        /// The transition will complete in about `512 / speed` frames
        /// (each frame is currently 100ms, so speed of `51` is about 1 second and `26` 0.5 second).
        /// As a special case, if speed is `0` the transition is immediate.
        /// If MCU is not capable of executing transitions, it can consider `speed` to be always `0`.
        /// If a monochrome LEDs is fitted, the average value of `red`, `green`, `blue` is used.
        /// If intensity of a monochrome LED cannot be controlled, any value larger than `0` should be considered
        /// on, and `0` (for all three channels) should be considered off.
        ///
        /// ```
        /// const [toRed, toGreen, toBlue, speed] = jdunpack<[number, number, number, number]>(buf, "u8 u8 u8 u8")
        /// ```
        /// </summary>
        SetStatusLight = 0x84,

        /// <summary>
        /// No args. Force client device into proxy mode.
        /// </summary>
        Proxy = 0x85,

        /// <summary>
        /// Argument: seed uint32_t. This opens a pipe to the device to provide an alternative, reliable transport of actions
        /// (and possibly other commands).
        /// The commands are wrapped as pipe data packets.
        /// Multiple invocations of this command with the same `seed` are dropped
        /// (and thus the command is not `unique`); otherwise `seed` carries no meaning
        /// and should be set to a random value by the client.
        /// Note that while the commands sends this way are delivered exactly once, the
        /// responses might get lost.
        ///
        /// ```
        /// const [seed] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        ReliableCommands = 0x86,

        /// <summary>
        /// report ReliableCommands
        /// ```
        /// const [commands] = jdunpack<[Uint8Array]>(buf, "b[12]")
        /// ```
        /// </summary>
    }

    public static class ControlCmdPack {
        /// <summary>
        /// Pack format for 'services' register data.
        /// </summary>
        public const string ServicesReport = "u16 u8 u8 r: u32";

        /// <summary>
        /// Pack format for 'flood_ping' register data.
        /// </summary>
        public const string FloodPing = "u32 u32 u8";

        /// <summary>
        /// Pack format for 'flood_ping' register data.
        /// </summary>
        public const string FloodPingReport = "u32 b";

        /// <summary>
        /// Pack format for 'set_status_light' register data.
        /// </summary>
        public const string SetStatusLight = "u8 u8 u8 u8";

        /// <summary>
        /// Pack format for 'reliable_commands' register data.
        /// </summary>
        public const string ReliableCommands = "u32";

        /// <summary>
        /// Pack format for 'reliable_commands' register data.
        /// </summary>
        public const string ReliableCommandsReport = "b[12]";
    }


    /// <summary>
    /// pipe_command WrappedCommand
    /// ```
    /// const [serviceSize, serviceIndex, serviceCommand, payload] = jdunpack<[number, number, number, Uint8Array]>(buf, "u8 u8 u16 b")
    /// ```
    /// </summary>


    public static class ControlinfoPack {
        /// <summary>
        /// Pack format for 'wrapped_command' register data.
        /// </summary>
        public const string WrappedCommand = "u8 u8 u16 b";
    }

    public enum ControlReg : ushort {
        /// <summary>
        /// Read-write μs uint32_t. When set to value other than `0`, it asks the device to reset after specified number of microseconds.
        /// This is typically used to implement watchdog functionality, where a brain device sets `reset_in` to
        /// say 1.6s every 0.5s.
        ///
        /// ```
        /// const [resetIn] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        ResetIn = 0x80,

        /// <summary>
        /// Constant string (bytes). Identifies the type of hardware (eg., ACME Corp. Servo X-42 Rev C)
        ///
        /// ```
        /// const [deviceDescription] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        DeviceDescription = 0x180,

        /// <summary>
        /// Constant uint32_t. A numeric code for the string above; used to identify firmware images and devices.
        ///
        /// ```
        /// const [productIdentifier] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        ProductIdentifier = 0x181,

        /// <summary>
        /// Constant uint32_t. Typically the same as `product_identifier` unless device was flashed by hand; the bootloader will respond to that code.
        ///
        /// ```
        /// const [bootloaderProductIdentifier] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        BootloaderProductIdentifier = 0x184,

        /// <summary>
        /// Constant string (bytes). A string describing firmware version; typically semver.
        ///
        /// ```
        /// const [firmwareVersion] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        FirmwareVersion = 0x185,

        /// <summary>
        /// Read-only °C int16_t. MCU temperature in degrees Celsius (approximate).
        ///
        /// ```
        /// const [mcuTemperature] = jdunpack<[number]>(buf, "i16")
        /// ```
        /// </summary>
        McuTemperature = 0x182,

        /// <summary>
        /// Read-only μs uint64_t. Number of microseconds since boot.
        ///
        /// ```
        /// const [uptime] = jdunpack<[number]>(buf, "u64")
        /// ```
        /// </summary>
        Uptime = 0x186,
    }

    public static class ControlRegPack {
        /// <summary>
        /// Pack format for 'reset_in' register data.
        /// </summary>
        public const string ResetIn = "u32";

        /// <summary>
        /// Pack format for 'device_description' register data.
        /// </summary>
        public const string DeviceDescription = "s";

        /// <summary>
        /// Pack format for 'product_identifier' register data.
        /// </summary>
        public const string ProductIdentifier = "u32";

        /// <summary>
        /// Pack format for 'bootloader_product_identifier' register data.
        /// </summary>
        public const string BootloaderProductIdentifier = "u32";

        /// <summary>
        /// Pack format for 'firmware_version' register data.
        /// </summary>
        public const string FirmwareVersion = "s";

        /// <summary>
        /// Pack format for 'mcu_temperature' register data.
        /// </summary>
        public const string McuTemperature = "i16";

        /// <summary>
        /// Pack format for 'uptime' register data.
        /// </summary>
        public const string Uptime = "u64";
    }

}
