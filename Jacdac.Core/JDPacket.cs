using System.Diagnostics;

namespace Jacdac
{
    [Flags]
    public enum PacketFlags
    {
        CommandPacket = 0x01,
        AckRequested = 0x02,
        Multicast = 0x04,
        vNext = 0x80
    }

    public struct JDPacket
    {
        public enum CommandType
        {
            Action,
            RegisterRead,
            RegisterWrite,
            Reserved,
            Event
        }

        public PacketFlags Flags { get; set; }

        public ulong DeviceIdentifier { get; set; }

        public byte ServiceSize { get; set; }

        public byte ServiceIndex { get; set; }

        public ushort ServiceCommand { get; set; }

        public byte[] Data { get; set; }

        public DateTime Timestamp { get; set; }

        public bool IsReport => !Flags.HasFlag(PacketFlags.CommandPacket);

        public bool IsAck => ServiceIndex == 0x3f;

        public bool IsAnnouncement => ServiceIndex == 0 && ServiceCommand == 0 && IsReport;

        public bool RequiresAck => Flags.HasFlag(PacketFlags.AckRequested);

        public byte EventCounter => (byte)((ServiceCommand >> 8) & 0x7F);

        public byte EventCode => (byte)(ServiceCommand & 0xFF);

        public CommandType OperationType => (ServiceCommand >> 12) switch
        {
            var x when x == 0 => CommandType.Action,
            var x when x == 1 => CommandType.RegisterRead,
            var x when x == 2 => CommandType.RegisterWrite,
            var x when x >= 8 && !IsAck => CommandType.Event,
            _ => CommandType.Reserved,

        };

        public ushort OperationCode => (ushort)(ServiceCommand & 0xFFF);

        public JDPacket(ulong deviceIdentifier, byte serviceIndex, ushort command, byte[]? data = null, PacketFlags flags = PacketFlags.CommandPacket)
        {
            DeviceIdentifier = deviceIdentifier;
            ServiceIndex = serviceIndex;
            ServiceCommand = command;
            ServiceSize = (byte)(data != null ? data.Length : 0);
            Data = data != null ? data : Array.Empty<byte>();
            Flags = flags;
            Timestamp = DateTime.Now;
        }

        public static JDPacket Parse(byte[] data)
        {
            var packet = new JDPacket();
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                packet.Flags = (PacketFlags)br.ReadByte();
                packet.DeviceIdentifier = br.ReadUInt64();
                packet.ServiceSize = br.ReadByte();
                packet.ServiceIndex = br.ReadByte();
                packet.ServiceCommand = br.ReadUInt16();
                packet.Data = br.ReadBytes(packet.ServiceSize);
                packet.Timestamp = DateTime.Now;
            }
            if(!packet.IsAck && !packet.IsAnnouncement)
                Debug.WriteLine(packet.ToString());
            return packet;
        }

        public override string ToString()
        {
            return $"[Packet] Device Identifier: {DeviceIdentifier}, ServiceIndex: {ServiceIndex}, ServiceCommand: {ServiceCommand}, Flags: {Flags.ToString()}, PayloadSize: {Data.Length}";
        }

        public delegate void JacdacPacketEventHandler(JDPacket packet);
    }

    public struct JDFrame
    {
        public byte[] Bytes { get; private set; }

        public byte[] Data => Bytes.Skip(12).ToArray();

        public byte FrameSize => Bytes[2];

        public PacketFlags Flags => (PacketFlags)Bytes[3];

        public bool RequiresAck => Flags.HasFlag(PacketFlags.AckRequested);

        public ushort ActualCRC => BitConverter.ToUInt16(Bytes);

        public ushort ExpectedCRC => CalculateCRC(Bytes.Skip(2).ToArray());

        public ulong DeviceIdentifier => BitConverter.ToUInt64(Bytes, 4);

        public JDFrame(byte[] data)
        {
            Bytes = data;
        }

        public static JDFrame FromPacket(JDPacket packet)
        {
            return FromPackets(new JDPacket[] { packet });
        }

        public static JDFrame FromPackets(JDPacket[] packets)
        {
            var firstPacketData = packets[0];
            if (packets.Any(p => packets[0].DeviceIdentifier != p.DeviceIdentifier || packets[0].Flags != p.Flags))
                throw new ArgumentException("All packets have to have the same device id and flags");

            var frameSize = (packets.Sum(p => p.Data.Length + 4));

            if (frameSize == 0 || frameSize > 240)
                throw new ArgumentException("Frame size too large");

            byte frameFlags = (byte)packets[0].Flags;

            byte[] frameData;
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.Write((ushort)1);        // frame_crc placeholder
                bw.Write((byte)frameSize);  // frame_size
                bw.Write(frameFlags);       // frame_flags
                bw.Write(packets[0].DeviceIdentifier);

                foreach (var packet in packets)
                {
                    bw.Write(packet.ServiceSize);
                    bw.Write(packet.ServiceIndex);
                    bw.Write(packet.ServiceCommand);
                    bw.Write(packet.Data);
                }

                frameData = ms.ToArray();
            }

            ushort crc = CalculateCRC(frameData.Skip(2).ToArray());
            frameData[0] = (byte)crc;
            frameData[1] = (byte)(crc >> 8);

            return new JDFrame(frameData);
        }

        public JDPacket[] ToPackets()
        {
            var packets = new List<JDPacket>();

            if (ExpectedCRC != ActualCRC)
            {
                Console.WriteLine($"Expected CRC {ExpectedCRC}, actual CRC {ActualCRC}");
                return Array.Empty<JDPacket>();
            }

            var commonData = Bytes.Skip(3).Take(9);
            for (var i = 12; i < FrameSize + 12;)
            {
                var packetSize = Bytes[i] + 4;
                packetSize = (packetSize + 3) & ~3;
                var packetData = commonData.Concat(Bytes.Skip(i).Take(packetSize)).ToArray();
                packets.Add(JDPacket.Parse(packetData));
                i += packetSize;
            }

            return packets.ToArray();
        }

        private static ushort CalculateCRC(byte[] data)
        {
            return NullFX.CRC.Crc16.ComputeChecksum(NullFX.CRC.Crc16Algorithm.CcittInitialValue0xFFFF, data);
        }
    }

}
