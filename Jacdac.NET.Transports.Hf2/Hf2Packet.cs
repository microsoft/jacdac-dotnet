using System;
using System.Linq;

namespace Jacdac.Transports.Hf2
{
    internal enum Hf2PacketType
    {
        InnerCommandPacket = 0,
        FinalCommandPacket = 1,
        SerialStdout = 2,
        SerialStderr = 3
    }
    internal sealed class Hf2Packet
    {
        public Hf2PacketType PacketType { get; private set; }

        public byte PayloadLength => (byte)Payload.Length;

        public byte[] Payload { get; private set; }

        public Hf2Packet(Hf2PacketType type, byte[] payload)
        {
            if (payload.Length > 63)
                throw new ArgumentException("Payload cannot be longer than 63 bytes");
            PacketType = type;
            Payload = payload;
        }

        public static Hf2Packet Parse(byte[] data)
        {
            if (data.Length == 0)
                throw new ArgumentException("No data supplied");

            Hf2PacketType type = (Hf2PacketType)(data[0] >> 6);
            byte payloadLength = (byte)(data[0] & 0x3F);
            byte[] payload = data.Skip(1).Take(payloadLength).ToArray();

            return new Hf2Packet(type, payload);
        }

        public byte[] ToByteArray()
        {
            byte[] output = new byte[64];
            output[0] = (byte)((int)PacketType << 6 | PayloadLength);
            for (var i = 1; i <= PayloadLength; i++)
                output[i] = Payload[i - 1];
            return output;
        }

        public override string ToString()
        {
            return $"{PacketType.ToString()} ({PayloadLength} bytes)";
        }
    }
}
