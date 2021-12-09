namespace Jacdac
{
    public static class Constants
    {
        public const int CMD_GET_REG = 0x1000;
        public const int CMD_SET_REG = 0x2000;

        public const int CMD_TOP_MASK = 0xf000;
        public const int CMD_REG_MASK = 0x0fff;


        // Commands 0x000-0x07f - common to all services
        // Commands 0x080-0xeff - defined per-service
        // Commands 0xf00-0xfff - reserved for implementation
        // enumeration data for CTRL, ad-data for other services
        public const int CMD_ADVERTISEMENT_DATA = 0x00;
        // event from sensor or on broadcast service
        public const int CMD_EVENT = 0x01;
        // request to calibrate sensor
        public const int CMD_CALIBRATE = 0x02;
        // request human-readable description of service
        public const int CMD_GET_DESCRIPTION = 0x03;

        // Commands specific to control service
        // do nothing
        public const int CMD_CTRL_NOOP = 0x80;
        // blink led or otherwise draw user's attention
        public const int CMD_CTRL_IDENTIFY = 0x81;
        // reset device
        public const int CMD_CTRL_RESET = 0x82;

        public const int STREAM_PORT_SHIFT = 7;
        public const int STREAM_COUNTER_MASK = 0x001f;
        public const int STREAM_CLOSE_MASK = 0x0020;
        public const int STREAM_METADATA_MASK = 0x0040;

        public const int JD_SERIAL_HEADER_SIZE = 16;
        public const int JD_SERIAL_MAX_PAYLOAD_SIZE = 236;
        public const int JD_SERVICE_NUMBER_MASK = 0x3f;
        public const int JD_SERVICE_NUMBER_INV_MASK = 0xc0;
        public const int JD_SERVICE_NUMBER_CRC_ACK = 0x3f;
        public const int JD_SERVICE_NUMBER_STREAM = 0x3e;
        public const int JD_SERVICE_NUMBER_CONTROL = 0x00;

        // the COMMAND flag signifies that the device_identifier is the recipent
        // (i.e., it's a command for the peripheral); the bit clear means device_identifier is the source
        // (i.e., it's a report from peripheral or a broadcast message)
        public const int JD_FRAME_FLAG_COMMAND = 0x01;
        // an ACK should be issued with CRC of this package upon reception
        public const int JD_FRAME_FLAG_ACK_REQUESTED = 0x02;
        // the device_identifier contains target service class number
        public const int JD_FRAME_FLAG_IDENTIFIER_IS_SERVICE_CLASS = 0x04;

        public const uint CMD_EVENT_MASK = 0x8000;
        public const uint CMD_EVENT_CODE_MASK = 0xff;
        public const uint CMD_EVENT_COUNTER_MASK = 0x7f;
        public const byte CMD_EVENT_COUNTER_POS = 8;

        public const uint UNDEFINED = 0xFFFFFFFF;
    }
}
