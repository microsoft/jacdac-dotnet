namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint ProtoTest = 0x16c7466a;
    }
    public enum ProtoTestReg : ushort {
        /// <summary>
        /// Read-write bool (uint8_t). A read write bool register.
        ///
        /// ```
        /// const [rwBool] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        RwBool = 0x81,

        /// <summary>
        /// Read-only bool (uint8_t). A read only bool register. Mirrors rw_bool.
        ///
        /// ```
        /// const [roBool] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        RoBool = 0x181,

        /// <summary>
        /// Read-write uint32_t. A read write u32 register.
        ///
        /// ```
        /// const [rwU32] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        RwU32 = 0x82,

        /// <summary>
        /// Read-only uint32_t. A read only u32 register.. Mirrors rw_u32.
        ///
        /// ```
        /// const [roU32] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        RoU32 = 0x182,

        /// <summary>
        /// Read-write int32_t. A read write i32 register.
        ///
        /// ```
        /// const [rwI32] = jdunpack<[number]>(buf, "i32")
        /// ```
        /// </summary>
        RwI32 = 0x83,

        /// <summary>
        /// Read-only int32_t. A read only i32 register.. Mirrors rw_i32.
        ///
        /// ```
        /// const [roI32] = jdunpack<[number]>(buf, "i32")
        /// ```
        /// </summary>
        RoI32 = 0x183,

        /// <summary>
        /// Read-write string (bytes). A read write string register.
        ///
        /// ```
        /// const [rwString] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        RwString = 0x84,

        /// <summary>
        /// Read-only string (bytes). A read only string register. Mirrors rw_string.
        ///
        /// ```
        /// const [roString] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        RoString = 0x184,

        /// <summary>
        /// Read-write bytes. A read write string register.
        ///
        /// ```
        /// const [rwBytes] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        RwBytes = 0x85,

        /// <summary>
        /// Read-only bytes. A read only string register. Mirrors ro_bytes.
        ///
        /// ```
        /// const [roBytes] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        RoBytes = 0x185,

        /// <summary>
        /// A read write i8, u8, u16, i32 register.
        ///
        /// ```
        /// const [i8, u8, u16, i32] = jdunpack<[number, number, number, number]>(buf, "i8 u8 u16 i32")
        /// ```
        /// </summary>
        RwI8U8U16I32 = 0x86,

        /// <summary>
        /// A read only i8, u8, u16, i32 register.. Mirrors rw_i8_u8_u16_i32.
        ///
        /// ```
        /// const [i8, u8, u16, i32] = jdunpack<[number, number, number, number]>(buf, "i8 u8 u16 i32")
        /// ```
        /// </summary>
        RoI8U8U16I32 = 0x186,

        /// <summary>
        /// A read write u8, string register.
        ///
        /// ```
        /// const [u8, str] = jdunpack<[number, string]>(buf, "u8 s")
        /// ```
        /// </summary>
        RwU8String = 0x87,

        /// <summary>
        /// A read only u8, string register.. Mirrors rw_u8_string.
        ///
        /// ```
        /// const [u8, str] = jdunpack<[number, string]>(buf, "u8 s")
        /// ```
        /// </summary>
        RoU8String = 0x187,
    }

    public static class ProtoTestRegPack {
        /// <summary>
        /// Pack format for 'rw_bool' register data.
        /// </summary>
        public const string RwBool = "u8";

        /// <summary>
        /// Pack format for 'ro_bool' register data.
        /// </summary>
        public const string RoBool = "u8";

        /// <summary>
        /// Pack format for 'rw_u32' register data.
        /// </summary>
        public const string RwU32 = "u32";

        /// <summary>
        /// Pack format for 'ro_u32' register data.
        /// </summary>
        public const string RoU32 = "u32";

        /// <summary>
        /// Pack format for 'rw_i32' register data.
        /// </summary>
        public const string RwI32 = "i32";

        /// <summary>
        /// Pack format for 'ro_i32' register data.
        /// </summary>
        public const string RoI32 = "i32";

        /// <summary>
        /// Pack format for 'rw_string' register data.
        /// </summary>
        public const string RwString = "s";

        /// <summary>
        /// Pack format for 'ro_string' register data.
        /// </summary>
        public const string RoString = "s";

        /// <summary>
        /// Pack format for 'rw_bytes' register data.
        /// </summary>
        public const string RwBytes = "b";

        /// <summary>
        /// Pack format for 'ro_bytes' register data.
        /// </summary>
        public const string RoBytes = "b";

        /// <summary>
        /// Pack format for 'rw_i8_u8_u16_i32' register data.
        /// </summary>
        public const string RwI8U8U16I32 = "i8 u8 u16 i32";

        /// <summary>
        /// Pack format for 'ro_i8_u8_u16_i32' register data.
        /// </summary>
        public const string RoI8U8U16I32 = "i8 u8 u16 i32";

        /// <summary>
        /// Pack format for 'rw_u8_string' register data.
        /// </summary>
        public const string RwU8String = "u8 s";

        /// <summary>
        /// Pack format for 'ro_u8_string' register data.
        /// </summary>
        public const string RoU8String = "u8 s";
    }

    public enum ProtoTestEvent : ushort {
        /// <summary>
        /// Argument: bo bool (uint8_t). An event raised when rw_bool is modified
        ///
        /// ```
        /// const [bo] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        EBool = 0x81,

        /// <summary>
        /// Argument: u32 uint32_t. An event raised when rw_u32 is modified
        ///
        /// ```
        /// const [u32] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        EU32 = 0x82,

        /// <summary>
        /// Argument: i32 int32_t. An event raised when rw_i32 is modified
        ///
        /// ```
        /// const [i32] = jdunpack<[number]>(buf, "i32")
        /// ```
        /// </summary>
        EI32 = 0x83,

        /// <summary>
        /// Argument: str string (bytes). An event raised when rw_string is modified
        ///
        /// ```
        /// const [str] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        EString = 0x84,

        /// <summary>
        /// Argument: bytes bytes. An event raised when rw_bytes is modified
        ///
        /// ```
        /// const [bytes] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        EBytes = 0x85,

        /// <summary>
        /// An event raised when rw_i8_u8_u16_i32 is modified
        ///
        /// ```
        /// const [i8, u8, u16, i32] = jdunpack<[number, number, number, number]>(buf, "i8 u8 u16 i32")
        /// ```
        /// </summary>
        EI8U8U16I32 = 0x86,

        /// <summary>
        /// An event raised when rw_u8_string is modified
        ///
        /// ```
        /// const [u8, str] = jdunpack<[number, string]>(buf, "u8 s")
        /// ```
        /// </summary>
        EU8String = 0x87,
    }

    public static class ProtoTestEventPack {
        /// <summary>
        /// Pack format for 'e_bool' register data.
        /// </summary>
        public const string EBool = "u8";

        /// <summary>
        /// Pack format for 'e_u32' register data.
        /// </summary>
        public const string EU32 = "u32";

        /// <summary>
        /// Pack format for 'e_i32' register data.
        /// </summary>
        public const string EI32 = "i32";

        /// <summary>
        /// Pack format for 'e_string' register data.
        /// </summary>
        public const string EString = "s";

        /// <summary>
        /// Pack format for 'e_bytes' register data.
        /// </summary>
        public const string EBytes = "b";

        /// <summary>
        /// Pack format for 'e_i8_u8_u16_i32' register data.
        /// </summary>
        public const string EI8U8U16I32 = "i8 u8 u16 i32";

        /// <summary>
        /// Pack format for 'e_u8_string' register data.
        /// </summary>
        public const string EU8String = "u8 s";
    }

    public enum ProtoTestCmd : ushort {
        /// <summary>
        /// Argument: bo bool (uint8_t). A command to set rw_bool.
        ///
        /// ```
        /// const [bo] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        CBool = 0x81,

        /// <summary>
        /// Argument: u32 uint32_t. A command to set rw_u32.
        ///
        /// ```
        /// const [u32] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        CU32 = 0x82,

        /// <summary>
        /// Argument: i32 int32_t. A command to set rw_i32.
        ///
        /// ```
        /// const [i32] = jdunpack<[number]>(buf, "i32")
        /// ```
        /// </summary>
        CI32 = 0x83,

        /// <summary>
        /// Argument: str string (bytes). A command to set rw_string.
        ///
        /// ```
        /// const [str] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        CString = 0x84,

        /// <summary>
        /// Argument: bytes bytes. A command to set rw_string.
        ///
        /// ```
        /// const [bytes] = jdunpack<[Uint8Array]>(buf, "b")
        /// ```
        /// </summary>
        CBytes = 0x85,

        /// <summary>
        /// A command to set rw_bytes.
        ///
        /// ```
        /// const [i8, u8, u16, i32] = jdunpack<[number, number, number, number]>(buf, "i8 u8 u16 i32")
        /// ```
        /// </summary>
        CI8U8U16I32 = 0x86,

        /// <summary>
        /// A command to set rw_u8_string.
        ///
        /// ```
        /// const [u8, str] = jdunpack<[number, string]>(buf, "u8 s")
        /// ```
        /// </summary>
        CU8String = 0x87,

        /// <summary>
        /// Argument: p_bytes pipe (bytes). A command to read the content of rw_bytes, byte per byte, as a pipe.
        ///
        /// ```
        /// const [pBytes] = jdunpack<[Uint8Array]>(buf, "b[12]")
        /// ```
        /// </summary>
        CReportPipe = 0x90,
    }

    public static class ProtoTestCmdPack {
        /// <summary>
        /// Pack format for 'c_bool' register data.
        /// </summary>
        public const string CBool = "u8";

        /// <summary>
        /// Pack format for 'c_u32' register data.
        /// </summary>
        public const string CU32 = "u32";

        /// <summary>
        /// Pack format for 'c_i32' register data.
        /// </summary>
        public const string CI32 = "i32";

        /// <summary>
        /// Pack format for 'c_string' register data.
        /// </summary>
        public const string CString = "s";

        /// <summary>
        /// Pack format for 'c_bytes' register data.
        /// </summary>
        public const string CBytes = "b";

        /// <summary>
        /// Pack format for 'c_i8_u8_u16_i32' register data.
        /// </summary>
        public const string CI8U8U16I32 = "i8 u8 u16 i32";

        /// <summary>
        /// Pack format for 'c_u8_string' register data.
        /// </summary>
        public const string CU8String = "u8 s";

        /// <summary>
        /// Pack format for 'c_report_pipe' register data.
        /// </summary>
        public const string CReportPipe = "b[12]";
    }


    /// <summary>
    /// pipe_report PBytes
    /// ```
    /// const [byte] = jdunpack<[number]>(buf, "u8")
    /// ```
    /// </summary>


    public static class ProtoTestinfoPack {
        /// <summary>
        /// Pack format for 'p_bytes' register data.
        /// </summary>
        public const string PBytes = "u8";
    }

}
