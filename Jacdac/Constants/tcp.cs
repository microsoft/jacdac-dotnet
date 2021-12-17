namespace Jacdac {
    // Service: TCP
    public static class TcpConstants
    {
        public const uint ServiceClass = 0x1b43b70b;
    }

    public enum TcpTcpError: int { // int32_t
        InvalidCommand = 0x1,
        InvalidCommandPayload = 0x2,
    }

    public enum TcpCmd {
        /**
         * Argument: inbound pipe (bytes). Open pair of pipes between network peripheral and a controlling device. In/outbound refers to direction from/to internet.
         *
         * ```
         * const [inbound] = jdunpack<[Uint8Array]>(buf, "b[12]")
         * ```
         */
        Open = 0x80,

        /**
         * report Open
         * ```
         * const [outboundPort] = jdunpack<[number]>(buf, "u16")
         * ```
         */
    }

    public static class TcpCmdPack {
        /**
         * Pack format for 'open' register data.
         */
        public const string Open = "b[12]";

        /**
         * Pack format for 'open' register data.
         */
        public const string OpenReport = "u16";
    }

    public enum TcpPipeCmd {
        /**
         * Open an SSL connection to a given host:port pair. Can be issued only once on given pipe.
         * After the connection is established, an empty data report is sent.
         * Connection is closed by closing the pipe.
         *
         * ```
         * const [tcpPort, hostname] = jdunpack<[number, string]>(buf, "u16 s")
         * ```
         */
        OpenSsl = 0x1,

        /**
         * Argument: error TcpError (int32_t). Reported when an error is encountered. Negative error codes come directly from the SSL implementation.
         *
         * ```
         * const [error] = jdunpack<[TcpTcpError]>(buf, "i32")
         * ```
         */
        Error = 0x0,
    }

    public static class TcpPipeCmdPack {
        /**
         * Pack format for 'open_ssl' register data.
         */
        public const string OpenSsl = "u16 s";

        /**
         * Pack format for 'error' register data.
         */
        public const string Error = "i32";
    }


    /**
     * pipe_command Outdata
     * ```
     * const [data] = jdunpack<[Uint8Array]>(buf, "b")
     * ```
     */

    /**
     * pipe_report Indata
     * ```
     * const [data] = jdunpack<[Uint8Array]>(buf, "b")
     * ```
     */


    public static class TcpinfoPack {
        /**
         * Pack format for 'outdata' register data.
         */
        public const string Outdata = "b";

        /**
         * Pack format for 'indata' register data.
         */
        public const string Indata = "b";
    }

}
