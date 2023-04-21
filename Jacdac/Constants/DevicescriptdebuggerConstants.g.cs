namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint DevsDbg = 0x155b5b40;
    }

    public enum DevsDbgValueTag: byte { // uint8_t
        Number = 0x1,
        Special = 0x2,
        Fiber = 0x3,
        BuiltinObject = 0x5,
        Exotic = 0x6,
        Unhandled = 0x7,
        ImgBuffer = 0x20,
        ImgStringBuiltin = 0x21,
        ImgStringAscii = 0x22,
        ImgStringUTF8 = 0x23,
        ImgRole = 0x30,
        ImgFunction = 0x31,
        ImgRoleMember = 0x32,
        ObjArray = 0x51,
        ObjMap = 0x52,
        ObjBuffer = 0x53,
        ObjString = 0x54,
        ObjStackFrame = 0x55,
        ObjPacket = 0x56,
        ObjBoundFunction = 0x57,
        ObjOpaque = 0x58,
        ObjAny = 0x50,
        ObjMask = 0xf0,
        User1 = 0xf1,
        User2 = 0xf2,
        User3 = 0xf3,
        User4 = 0xf4,
    }


    public enum DevsDbgValueSpecial: byte { // uint8_t
        Undefined = 0x0,
        True = 0x1,
        False = 0x2,
        Null = 0x3,
        Globals = 0x64,
        CurrentException = 0x65,
    }


    public enum DevsDbgFunIdx: ushort { // uint16_t
        None = 0x0,
        Main = 0xc34f,
        FirstBuiltIn = 0xc350,
    }


    public enum DevsDbgFiberHandle: uint { // uint32_t
        None = 0x0,
    }


    public enum DevsDbgProgramCounter: uint { // uint32_t
    }


    public enum DevsDbgObjStackFrame: uint { // uint32_t
        Null = 0x0,
    }


    public enum DevsDbgString: uint { // uint32_t
        StaticIndicatorMask = 0x80000001,
        StaticTagMask = 0x7f000000,
        StaticIndexMask = 0xfffffe,
        Unhandled = 0x0,
    }


    [System.Flags]
    public enum DevsDbgStepFlags: ushort { // uint16_t
        StepOut = 0x1,
        StepIn = 0x2,
        Throw = 0x4,
    }


    public enum DevsDbgSuspensionType: byte { // uint8_t
        None = 0x0,
        Breakpoint = 0x1,
        UnhandledException = 0x2,
        HandledException = 0x3,
        Halt = 0x4,
        Panic = 0x5,
        Restart = 0x6,
        DebuggerStmt = 0x7,
        Step = 0x8,
    }

    public enum DevsDbgCmd : ushort {
        /// <summary>
        /// Argument: results pipe (bytes). List the currently running fibers (threads).
        ///
        /// ```
        /// const [results] = jdunpack<[Uint8Array]>(buf, "b[12]")
        /// ```
        /// </summary>
        ReadFibers = 0x80,

        /// <summary>
        /// List stack frames in a fiber.
        ///
        /// ```
        /// const [results, fiberHandle] = jdunpack<[Uint8Array, DevsDbgFiberHandle]>(buf, "b[12] u32")
        /// ```
        /// </summary>
        ReadStack = 0x81,

        /// <summary>
        /// Read variable slots in a stack frame, elements of an array, etc.
        ///
        /// ```
        /// const [results, v0, tag, start, length] = jdunpack<[Uint8Array, number, DevsDbgValueTag, number, number]>(buf, "b[12] u32 u8 x[1] u16 u16")
        /// ```
        /// </summary>
        ReadIndexedValues = 0x82,

        /// <summary>
        /// Read variable slots in an object.
        ///
        /// ```
        /// const [results, v0, tag] = jdunpack<[Uint8Array, number, DevsDbgValueTag]>(buf, "b[12] u32 u8")
        /// ```
        /// </summary>
        ReadNamedValues = 0x83,

        /// <summary>
        /// Read a specific value.
        ///
        /// ```
        /// const [v0, tag] = jdunpack<[number, DevsDbgValueTag]>(buf, "u32 u8")
        /// ```
        /// </summary>
        ReadValue = 0x84,

        /// <summary>
        /// report ReadValue
        /// ```
        /// const [v0, v1, fnIdx, tag] = jdunpack<[number, number, DevsDbgFunIdx, DevsDbgValueTag]>(buf, "u32 u32 u16 u8")
        /// ```
        /// </summary>

        /// <summary>
        /// Read bytes of a string (UTF8) or buffer value.
        ///
        /// ```
        /// const [results, v0, tag, start, length] = jdunpack<[Uint8Array, number, DevsDbgValueTag, number, number]>(buf, "b[12] u32 u8 x[1] u16 u16")
        /// ```
        /// </summary>
        ReadBytes = 0x85,

        /// <summary>
        /// Set breakpoint(s) at a location(s).
        ///
        /// ```
        /// const [breakPc] = jdunpack<[DevsDbgProgramCounter[]]>(buf, "u32[]")
        /// ```
        /// </summary>
        SetBreakpoints = 0x90,

        /// <summary>
        /// Clear breakpoint(s) at a location(s).
        ///
        /// ```
        /// const [breakPc] = jdunpack<[DevsDbgProgramCounter[]]>(buf, "u32[]")
        /// ```
        /// </summary>
        ClearBreakpoints = 0x91,

        /// <summary>
        /// No args. Clear all breakpoints.
        /// </summary>
        ClearAllBreakpoints = 0x92,

        /// <summary>
        /// No args. Resume program execution after a breakpoint was hit.
        /// </summary>
        Resume = 0x93,

        /// <summary>
        /// No args. Try suspending current program. Client needs to wait for `suspended` event afterwards.
        /// </summary>
        Halt = 0x94,

        /// <summary>
        /// No args. Run the program from the beginning and halt on first instruction.
        /// </summary>
        RestartAndHalt = 0x95,

        /// <summary>
        /// Set breakpoints that only trigger in the specified stackframe and resume program.
        /// The breakpoints are cleared automatically on next suspension (regardless of the reason).
        ///
        /// ```
        /// const [stackframe, flags, breakPc] = jdunpack<[DevsDbgObjStackFrame, DevsDbgStepFlags, DevsDbgProgramCounter[]]>(buf, "u32 u16 x[2] u32[]")
        /// ```
        /// </summary>
        Step = 0x96,
    }

    public static class DevsDbgCmdPack {
        /// <summary>
        /// Pack format for 'read_fibers' data.
        /// </summary>
        public const string ReadFibers = "b[12]";

        /// <summary>
        /// Pack format for 'read_stack' data.
        /// </summary>
        public const string ReadStack = "b[12] u32";

        /// <summary>
        /// Pack format for 'read_indexed_values' data.
        /// </summary>
        public const string ReadIndexedValues = "b[12] u32 u8 u8 u16 u16";

        /// <summary>
        /// Pack format for 'read_named_values' data.
        /// </summary>
        public const string ReadNamedValues = "b[12] u32 u8";

        /// <summary>
        /// Pack format for 'read_value' data.
        /// </summary>
        public const string ReadValue = "u32 u8";

        /// <summary>
        /// Pack format for 'read_value' data.
        /// </summary>
        public const string ReadValueReport = "u32 u32 u16 u8";

        /// <summary>
        /// Pack format for 'read_bytes' data.
        /// </summary>
        public const string ReadBytes = "b[12] u32 u8 u8 u16 u16";

        /// <summary>
        /// Pack format for 'set_breakpoints' data.
        /// </summary>
        public const string SetBreakpoints = "r: u32";

        /// <summary>
        /// Pack format for 'clear_breakpoints' data.
        /// </summary>
        public const string ClearBreakpoints = "r: u32";

        /// <summary>
        /// Pack format for 'step' data.
        /// </summary>
        public const string Step = "u32 u16 u16 r: u32";
    }

    public enum DevsDbgPipe : ushort {
        /// <summary>
        /// pipe_report Fiber
        /// ```
        /// const [handle, initialFn, currFn] = jdunpack<[DevsDbgFiberHandle, DevsDbgFunIdx, DevsDbgFunIdx]>(buf, "u32 u16 u16")
        /// ```
        /// </summary>

        /// <summary>
        /// pipe_report Stackframe
        /// ```
        /// const [self, pc, closure, fnIdx] = jdunpack<[DevsDbgObjStackFrame, DevsDbgProgramCounter, DevsDbgObjStackFrame, DevsDbgFunIdx]>(buf, "u32 u32 u32 u16 x[2]")
        /// ```
        /// </summary>

        /// <summary>
        /// pipe_report Value
        /// ```
        /// const [v0, v1, fnIdx, tag] = jdunpack<[number, number, DevsDbgFunIdx, DevsDbgValueTag]>(buf, "u32 u32 u16 u8")
        /// ```
        /// </summary>

        /// <summary>
        /// pipe_report KeyValue
        /// ```
        /// const [key, v0, v1, fnIdx, tag] = jdunpack<[DevsDbgString, number, number, DevsDbgFunIdx, DevsDbgValueTag]>(buf, "u32 u32 u32 u16 u8")
        /// ```
        /// </summary>

        /// <summary>
        /// pipe_report BytesValue
        /// ```
        /// const [data] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
    }

    public static class DevsDbgPipePack {
        /// <summary>
        /// Pack format for 'fiber' data.
        /// </summary>
        public const string Fiber = "u32 u16 u16";

        /// <summary>
        /// Pack format for 'stackframe' data.
        /// </summary>
        public const string Stackframe = "u32 u32 u32 u16 u16";

        /// <summary>
        /// Pack format for 'value' data.
        /// </summary>
        public const string Value = "u32 u32 u16 u8";

        /// <summary>
        /// Pack format for 'key_value' data.
        /// </summary>
        public const string KeyValue = "u32 u32 u32 u16 u8";

        /// <summary>
        /// Pack format for 'bytes_value' data.
        /// </summary>
        public const string BytesValue = "b";
    }

    public enum DevsDbgReg : ushort {
        /// <summary>
        /// Read-write bool (uint8_t). Turn on/off the debugger interface.
        ///
        /// ```
        /// const [enabled] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Enabled = 0x1,

        /// <summary>
        /// Read-write bool (uint8_t). Wheather to place breakpoint at unhandled exception.
        ///
        /// ```
        /// const [breakAtUnhandledExn] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        BreakAtUnhandledExn = 0x80,

        /// <summary>
        /// Read-write bool (uint8_t). Wheather to place breakpoint at handled exception.
        ///
        /// ```
        /// const [breakAtHandledExn] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        BreakAtHandledExn = 0x81,

        /// <summary>
        /// Read-only bool (uint8_t). Indicates if the program is currently suspended.
        /// Most commands can only be executed when the program is suspended.
        ///
        /// ```
        /// const [isSuspended] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        IsSuspended = 0x180,
    }

    public static class DevsDbgRegPack {
        /// <summary>
        /// Pack format for 'enabled' data.
        /// </summary>
        public const string Enabled = "u8";

        /// <summary>
        /// Pack format for 'break_at_unhandled_exn' data.
        /// </summary>
        public const string BreakAtUnhandledExn = "u8";

        /// <summary>
        /// Pack format for 'break_at_handled_exn' data.
        /// </summary>
        public const string BreakAtHandledExn = "u8";

        /// <summary>
        /// Pack format for 'is_suspended' data.
        /// </summary>
        public const string IsSuspended = "u8";
    }

    public enum DevsDbgEvent : ushort {
        /// <summary>
        /// Emitted when the program hits a breakpoint or similar event in the specified fiber.
        ///
        /// ```
        /// const [fiber, type] = jdunpack<[DevsDbgFiberHandle, DevsDbgSuspensionType]>(buf, "u32 u8")
        /// ```
        /// </summary>
        Suspended = 0x80,
    }

    public static class DevsDbgEventPack {
        /// <summary>
        /// Pack format for 'suspended' data.
        /// </summary>
        public const string Suspended = "u32 u8";
    }

}
