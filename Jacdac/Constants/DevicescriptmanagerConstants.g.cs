namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint DeviceScriptManager = 0x1134ea2b;
    }
    public enum DeviceScriptManagerCmd : ushort {
        /// <summary>
        /// Argument: bytecode_size B uint32_t. Open pipe for streaming in the bytecode of the program. The size of the bytecode has to be declared upfront.
        /// To clear the program, use `bytecode_size == 0`.
        /// The bytecode is streamed over regular pipe data packets.
        /// The bytecode shall be fully written into flash upon closing the pipe.
        /// If `autostart` is true, the program will start after being deployed.
        /// The data payloads, including the last one, should have a size that is a multiple of 32 bytes.
        /// Thus, the initial bytecode_size also needs to be a multiple of 32.
        ///
        /// ```
        /// const [bytecodeSize] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        DeployBytecode = 0x80,

        /// <summary>
        /// report DeployBytecode
        /// ```
        /// const [bytecodePort] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>

        /// <summary>
        /// Argument: bytecode pipe (bytes). Get the current bytecode deployed on device.
        ///
        /// ```
        /// const [bytecode] = jdunpack<[Uint8Array]>(buf, "b[12]")
        /// ```
        /// </summary>
        ReadBytecode = 0x81,
    }

    public static class DeviceScriptManagerCmdPack {
        /// <summary>
        /// Pack format for 'deploy_bytecode' data.
        /// </summary>
        public const string DeployBytecode = "u32";

        /// <summary>
        /// Pack format for 'deploy_bytecode' data.
        /// </summary>
        public const string DeployBytecodeReport = "u16";

        /// <summary>
        /// Pack format for 'read_bytecode' data.
        /// </summary>
        public const string ReadBytecode = "b[12]";
    }

    public enum DeviceScriptManagerPipe : ushort {
        /// <summary>
        /// pipe_report Bytecode
        /// ```
        /// const [data] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
    }

    public static class DeviceScriptManagerPipePack {
        /// <summary>
        /// Pack format for 'bytecode' data.
        /// </summary>
        public const string Bytecode = "b";
    }

    public enum DeviceScriptManagerReg : ushort {
        /// <summary>
        /// Read-write bool (uint8_t). Indicates if the program is currently running.
        /// To restart the program, stop it (write `0`), read back the register to make sure it's stopped,
        /// start it, and read back.
        ///
        /// ```
        /// const [running] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Running = 0x80,

        /// <summary>
        /// Read-write bool (uint8_t). Indicates wheather the program should be re-started upon `reboot()` or `panic()`.
        /// Defaults to `true`.
        ///
        /// ```
        /// const [autostart] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Autostart = 0x81,

        /// <summary>
        /// Read-only uint32_t. The size of current program.
        ///
        /// ```
        /// const [programSize] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        ProgramSize = 0x180,

        /// <summary>
        /// Read-only uint32_t. Return FNV1A hash of the current bytecode.
        ///
        /// ```
        /// const [programHash] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        ProgramHash = 0x181,

        /// <summary>
        /// Read-only bytes. Return 32-byte long SHA-256 hash of the current bytecode.
        ///
        /// ```
        /// const [programSha256] = jdunpack<[Uint8Array]>(buf, "b[32]")
        /// ```
        /// </summary>
        ProgramSha256 = 0x182,

        /// <summary>
        /// Returns the runtime version number compatible with [Semver](https://semver.org/).
        /// When read as 32-bit little endian integer a version `7.15.500` would be `0x07_0F_01F4`.
        ///
        /// ```
        /// const [patch, minor, major] = jdunpack<[number, number, number]>(buf, "u16 u8 u8")
        /// ```
        /// </summary>
        RuntimeVersion = 0x183,

        /// <summary>
        /// Read-only string (bytes). The name of currently running program. The compiler takes is from `package.json`.
        ///
        /// ```
        /// const [programName] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        ProgramName = 0x184,

        /// <summary>
        /// Read-only string (bytes). The version number of currently running program. The compiler takes is from `package.json`
        /// and `git`.
        ///
        /// ```
        /// const [programVersion] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        ProgramVersion = 0x185,
    }

    public static class DeviceScriptManagerRegPack {
        /// <summary>
        /// Pack format for 'running' data.
        /// </summary>
        public const string Running = "u8";

        /// <summary>
        /// Pack format for 'autostart' data.
        /// </summary>
        public const string Autostart = "u8";

        /// <summary>
        /// Pack format for 'program_size' data.
        /// </summary>
        public const string ProgramSize = "u32";

        /// <summary>
        /// Pack format for 'program_hash' data.
        /// </summary>
        public const string ProgramHash = "u32";

        /// <summary>
        /// Pack format for 'program_sha256' data.
        /// </summary>
        public const string ProgramSha256 = "b[32]";

        /// <summary>
        /// Pack format for 'runtime_version' data.
        /// </summary>
        public const string RuntimeVersion = "u16 u8 u8";

        /// <summary>
        /// Pack format for 'program_name' data.
        /// </summary>
        public const string ProgramName = "s";

        /// <summary>
        /// Pack format for 'program_version' data.
        /// </summary>
        public const string ProgramVersion = "s";
    }

    public enum DeviceScriptManagerEvent : ushort {
        /// <summary>
        /// Emitted when the program calls `panic(panic_code)` or `reboot()` (`panic_code == 0` in that case).
        /// The byte offset in byte code of the call is given in `program_counter`.
        /// The program will restart immediately when `panic_code == 0` or in a few seconds otherwise.
        ///
        /// ```
        /// const [panicCode, programCounter] = jdunpack<[number, number]>(buf, "u32 u32")
        /// ```
        /// </summary>
        ProgramPanic = 0x80,

        /// <summary>
        /// Emitted after bytecode of the program has changed.
        /// </summary>
        ProgramChange = 0x3,
    }

    public static class DeviceScriptManagerEventPack {
        /// <summary>
        /// Pack format for 'program_panic' data.
        /// </summary>
        public const string ProgramPanic = "u32 u32";
    }

}
