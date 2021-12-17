namespace Jacdac
{
    public static class Constants
    {
        public const ushort CMD_GET_REG = 0x1000;
        public const ushort CMD_SET_REG = 0x2000;

        public const uint CMD_TOP_MASK = 0xf000;
        public const uint CMD_REG_MASK = 0x0fff;


        // Commands 0x000-0x07f - common to all services
        // Commands 0x080-0xeff - defined per-service
        // Commands 0xf00-0xfff - reserved for implementation
        // enumeration data for CTRL, ad-data for other services
        public const ushort CMD_ADVERTISEMENT_DATA = 0x00;
        // event from sensor or on broadcast service
        public const ushort CMD_EVENT = 0x01;
        // request to calibrate sensor
        public const ushort CMD_CALIBRATE = 0x02;
        // request human-readable description of service
        public const ushort CMD_GET_DESCRIPTION = 0x03;
        // command not available
        public const ushort CMD_NOT_IMPLEMENTED = 0x3;

        // Commands specific to control service
        // do nothing
        public const ushort CMD_CTRL_NOOP = 0x80;
        // blink led or otherwise draw user's attention
        public const ushort CMD_CTRL_IDENTIFY = 0x81;
        // reset device
        public const ushort CMD_CTRL_RESET = 0x82;

        public const byte PIPE_PORT_SHIFT = 7;
        public const ushort PIPE_COUNTER_MASK = 0x001f;
        public const ushort PIPE_CLOSE_MASK = 0x0020;
        public const ushort PIPE_METADATA_MASK = 0x0040;

        public const uint STREAM_PORT_SHIFT = 7;
        public const uint STREAM_COUNTER_MASK = 0x001f;
        public const uint STREAM_CLOSE_MASK = 0x0020;
        public const uint STREAM_METADATA_MASK = 0x0040;

        public const uint JD_SERIAL_HEADER_SIZE = 16;
        public const uint JD_SERIAL_MAX_PAYLOAD_SIZE = 236;
        public const byte JD_SERVICE_INDEX_MASK = 0x3f;
        public const byte JD_SERVICE_INDEX_INV_MASK = 0xc0;
        public const byte JD_SERVICE_INDEX_CRC_ACK = 0x3f;
        public const byte JD_SERVICE_INDEX_PIPE = 0x3e;
        public const byte JD_SERVICE_INDEX_MAX_NORMAL = 0x30;
        public const byte JD_SERVICE_INDEX_CTRL = 0x00;

        // the COMMAND flag signifies that the device_identifier is the recipent
        // (i.e., it's a command for the peripheral); the bit clear means device_identifier is the source
        // (i.e., it's a report from peripheral or a broadcast message)
        public const byte JD_FRAME_FLAG_COMMAND = 0x01;
        // an ACK should be issued with CRC of this package upon reception
        public const byte JD_FRAME_FLAG_ACK_REQUESTED = 0x02;
        // the device_identifier contains target service class number
        public const uint JD_FRAME_FLAG_IDENTIFIER_IS_SERVICE_CLASS = 0x04;

        public const uint JD_ADVERTISEMENT_0_COUNTER_MASK = 0x0000000f;
        public const uint JD_ADVERTISEMENT_0_ACK_SUPPORTED = 0x00000100;

        public const uint CMD_EVENT_MASK = 0x8000;
        public const uint CMD_EVENT_CODE_MASK = 0xff;
        public const uint CMD_EVENT_COUNTER_MASK = 0x7f;
        public const byte CMD_EVENT_COUNTER_POS = 8;

        public const ushort CONTROL_REG_RESET_IN = 0x80;

        public const uint UNDEFINED = 0xFFFFFFFF;

        public const uint RESET_IN_TIME_US = 2000000;
        // time withouth seeing a package to be considered "lost", 2x announce interval
        public const uint JD_DEVICE_LOST_DELAY = 1500;
        // time without seeing a packet to be considered "disconnected"
        public const uint JD_DEVICE_DISCONNECTED_DELAY = 5000;

        public const uint REGISTER_POLL_STREAMING_INTERVAL = 5000;
        public const uint REGISTER_POLL_REPORT_MAX_INTERVAL = 60000;
        public const uint REGISTER_POLL_FIRST_REPORT_INTERVAL = 400;
        public const uint REGISTER_POLL_REPORT_INTERVAL = 5001;
    }
}
