namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint Tcp = 0x1b43b70b;
    }

    public enum TcpTcpError: int { // int32_t
        InvalidCommand = 0x1,
        InvalidCommandPayload = 0x2,
    }

    public enum TcpCmd {
        /// <summary>
        /// Argument: inbound pipe (bytes). Open pair of pipes between network peripheral and a controlling device. In/outbound refers to direction from/to internet.
        ///
        /// ```
        /// const [inbound] = jdunpack<[Uint8Array]>(buf, "b[12]")
        /// ```
        /// </summary>
        Open = 0x80,

        /// <summary>
        /// report Open
        /// ```
        /// const [outboundPort] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
    }

    public static class TcpCmdPack {
        /// <summary>
        /// Pack format for 'open' register data.
        /// </summary>
        public const string Open = "b[12]";

        /// <summary>
        /// Pack format for 'open' register data.
        /// </summary>
        public const string OpenReport = "u16";
    }

    public enum TcpPipeCmd {
        /// <summary>
        /// Open an SSL connection to a given host:port pair. Can be issued only once on given pipe.
        /// After the connection is established, an empty data report is sent.
        /// Connection is closed by closing the pipe.
        ///
        /// ```
        /// const [tcpPort, hostname] = jdunpack<[number, string]>(buf, "u16 s")
        /// ```
        /// </summary>
        OpenSsl = 0x1,

        /// <summary>
        /// Argument: error TcpError (int32_t). Reported when an error is encountered. Negative error codes come directly from the SSL implementation.
        ///
        /// ```
        /// const [error] = jdunpack<[TcpTcpError]>(buf, "i32")
        /// ```
        /// </summary>
        Error = 0x0,
    }

    public static class TcpPipeCmdPack {
        /// <summary>
        /// Pack format for 'open_ssl' register data.
        /// </summary>
        public const string OpenSsl = "u16 s";

        /// <summary>
        /// Pack format for 'error' register data.
        /// </summary>
        public const string Error = "i32";
    }


    /// <summary>
    /// pipe_command Outdata
    /// ```
    /// const [data] = jdunpack<[Uint8Array]>(buf, "b")
    /// ```
    /// </summary>

    /// <summary>
    /// pipe_report Indata
    /// ```
    /// const [data] = jdunpack<[Uint8Array]>(buf, "b")
    /// ```
    /// </summary>


    public static class TcpinfoPack {
        /// <summary>
        /// Pack format for 'outdata' register data.
        /// </summary>
        public const string Outdata = "b";

        /// <summary>
        /// Pack format for 'indata' register data.
        /// </summary>
        public const string Indata = "b";
    }

}
