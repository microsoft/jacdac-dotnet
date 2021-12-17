using System;
using System.Diagnostics;
using System.Collections;

namespace Jacdac
{
    [Serializable]
    public sealed class AckException : Exception
    {
        public readonly Packet Packet;

        public AckException(Packet packet)
        {
            this.Packet = packet;
        }
    }

    [Serializable]
    [DebuggerDisplay("{DeviceId}[{ServiceIndex}]>{ServiceCommand}")]
    public sealed class Packet
    {
        private readonly byte[] header;
        private readonly byte[] data;
        public TimeSpan Timestamp { get; set; }

        public static readonly Packet[] EmptyFrame = new Packet[0];

        private Packet(byte[] header, byte[] data)
        {
            Debug.Assert(header != null);
            Debug.Assert(data != null);
            Stats.PacketAllocated++;
            this.header = header;
            this.data = data;
            Debug.Assert(this.Size == this.Data.Length);
        }

        public override string ToString()
        {
            return HexEncoding.ToString(this.ToBuffer());
        }

        public static Packet FromBinary(byte[] buffer, bool fixSize = false)
        {
            var header = Util.Slice(buffer, 0, (int)Constants.JD_SERIAL_HEADER_SIZE);
            var data = Util.Slice(buffer, (int)Constants.JD_SERIAL_HEADER_SIZE);
            if (fixSize)
                header[12] = (byte)data.Length;
            var p = new Packet(header, data);
            return p;
        }

        public static Packet[] FromFrame(byte[] frame)
        {
            var size = frame.Length < 12 ? 0 : frame[2];
            if (frame.Length < size + 12)
            {
                Debug.WriteLine($"frame too short: got only {frame.Length} bytes; expecting {size + 12}");
                return Packet.EmptyFrame;

            }
            else if (size < 4)
            {
                Debug.WriteLine("empty packet");
                return Packet.EmptyFrame;
            }
            else
            {
                var computed = Platform.Crc16(frame, 2, size + 10);
                var actual = BitConverter.ToUInt16(frame, 0);
                if (actual != computed)
                {
                    Debug.WriteLine($"crc mismatch; sz={size} got:{actual}, exp:{computed}");
                    return Packet.EmptyFrame;
                }

                var res = new ArrayList(1);
                if (frame.Length != 12 + size)
                {
                    Debug.WriteLine($"unexpected packet len: ${frame.Length}");
                    return Packet.EmptyFrame;
                }

                for (var ptr = 12; ptr < 12 + size;)
                {
                    var psz = frame[ptr] + 4;
                    var sz = (psz + 3) & ~3; // align
                    if (ptr + psz > 12 + size)
                    {
                        Debug.WriteLine($"invalid frame compression, res len ={res.Count}");
                        break;
                    }
                    var p = Packet.FromFrameBinary(frame, ptr, psz);
                    res.Add(p);
                    // only set req_ack flag on first packet - otherwise we would sent multiple acks
                    if (res.Count > 1) p.RequiresAck = false;
                    ptr += sz;
                }

                return (Packet[])res.ToArray(typeof(Packet));
            }
        }

        private static Packet FromFrameBinary(byte[] frame, int ptr, int psz)
        {
            var header = new byte[Jacdac.Constants.JD_SERIAL_HEADER_SIZE];
            var data = new byte[psz - 4];
            Array.Copy(frame, 0, header, 0, 12);
            Array.Copy(frame, ptr, header, 12, 4);
            Array.Copy(frame, ptr + 4, data, 0, psz - 4);
            return new Packet(header, data);
        }

        public static byte[] ToFrame(Packet[] packets)
        {
            if (packets == null || packets.Length == 0)
                throw new ArgumentNullException("packets");
            var firstPacket = packets[0];
            var flags = firstPacket.FrameFlags;
            for (var i = 1; i < packets.Length; i++)
                if (!firstPacket.IsSameDevice(packets[i]) || packets[i].FrameFlags != flags)
                    throw new ArgumentException("All packets have to have the same device id and flags");

            uint frameSize = 0;
            for (var i = 0; i < packets.Length; i++)
                frameSize += (uint)packets[i].Size + 4;

            if (frameSize > Jacdac.Constants.JD_SERIAL_MAX_PAYLOAD_SIZE + 4)
                throw new ArgumentOutOfRangeException("Frame size too large");

            byte[] frameData = new byte[frameSize + 12];
            uint offset = 2; // // frame_crc placeholder
            frameData[offset++] = (byte)frameSize;
            frameData[offset++] = flags;
            Array.Copy(firstPacket.header, 4, frameData, (int)offset, 8);
            offset += 8;
            Debug.Assert(offset == 12);
            foreach (var packet in packets)
            {
                frameData[offset++] = packet.Size;
                frameData[offset++] = packet.ServiceIndex;
                Util.Write16(frameData, offset, packet.ServiceCommand);
                offset += 2;
                packet.Data.CopyTo(frameData, (int)offset);
                offset += (uint)packet.Data.Length;
            }
            Debug.Assert(offset == frameData.Length);

            ushort crc = Platform.Crc16(frameData, 2, frameData.Length - 2);
            Util.Write16(frameData, 0, crc);
            return frameData;
        }

        public static Packet From(ushort serviceCommand, byte[] buffer = null)
        {
            if (buffer == null)
                buffer = new byte[0];
            if (buffer.Length > Jacdac.Constants.JD_SERIAL_MAX_PAYLOAD_SIZE)
                throw new ArgumentOutOfRangeException("packet data length too large");
            var header = new byte[Jacdac.Constants.JD_SERIAL_HEADER_SIZE];
            header[12] = (byte)buffer.Length;
            var p = new Packet(
                header,
                buffer
            );
            p.ServiceCommand = serviceCommand;
            return p;
        }

        public static Packet FromCmd(ushort serviceCommand, byte[] buffer = null)
        {
            var pkt = From(serviceCommand, buffer);
            pkt.IsCommand = true;
            return pkt;
        }

        public byte[] ToBuffer() => Util.BufferConcat(this.header, this.data);

        public string ToTrace()
        {
            return $"{this.Timestamp.TotalMilliseconds}\t\t{HexEncoding.ToString(Packet.ToFrame(new Packet[] { this }))}";
        }

        public string DeviceId
        {
            get => HexEncoding.ToString(this.header, 4, 8);
            set
            {
                var idb = HexEncoding.ToBuffer(value);
                this.DeviceIdBuffer = idb;
            }
        }

        public byte[] DeviceIdBuffer
        {
            get
            {
                return Util.Slice(this.header, 4, 4 + 8);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("DeviceIdBuffer");
                if (value.Length != 8)
                    throw new ArgumentOutOfRangeException("DeviceIdBuffer");
                Array.Copy(value, 0, this.header, 4, value.Length);
            }
        }

        public bool IsSameDevice(Packet other)
        {
            var oheader = other.header;
            for (var i = 4; i < 12; i++)
            {
                if (this.header[i] != oheader[i])
                    return false;
            }
            return true;
        }

        public byte FrameFlags => this.header[3];

        public bool IsMultiCommand
        {
            get { return (this.FrameFlags & Jacdac.Constants.JD_FRAME_FLAG_IDENTIFIER_IS_SERVICE_CLASS) != 0; }
        }

        public void SetMultiCommand(uint serviceClass)
        {
            this.header[3] |= (byte)Jacdac.Constants.JD_FRAME_FLAG_IDENTIFIER_IS_SERVICE_CLASS | (byte)Jacdac.Constants.JD_FRAME_FLAG_COMMAND;
            Util.Write32(this.header, 4, serviceClass);
            Util.Write32(this.header, 8, 0);
        }

        public uint MulticommandClass
        {
            get
            {
                if ((this.FrameFlags & Jacdac.Constants.JD_FRAME_FLAG_IDENTIFIER_IS_SERVICE_CLASS) != 0)
                    return BitConverter.ToUInt32(this.header, 4);
                return Constants.UNDEFINED;
            }
        }

        public byte Size => this.header[12];

        public bool RequiresAck
        {
            get => (this.FrameFlags & Jacdac.Constants.JD_FRAME_FLAG_ACK_REQUESTED) != 0 ? true : false;

            set
            {
                if (value != this.RequiresAck)
                    this.header[3] ^= Jacdac.Constants.JD_FRAME_FLAG_ACK_REQUESTED;
            }
        }

        public bool IsCrcAck
        {
            get => this.ServiceIndex == Jacdac.Constants.JD_SERVICE_INDEX_CRC_ACK;
        }

        public bool IsPipe
        {
            get => this.ServiceIndex == Jacdac.Constants.JD_SERVICE_INDEX_PIPE;
        }

        public ushort PipePort
        {
            get => (ushort)(this.ServiceCommand >> Jacdac.Constants.PIPE_PORT_SHIFT);
        }

        public ushort pipeCount
        {
            get => (ushort)(this.ServiceCommand & Jacdac.Constants.PIPE_COUNTER_MASK);
        }

        public byte ServiceIndex
        {
            get => (byte)(this.header[13] & Jacdac.Constants.JD_SERVICE_INDEX_MASK);
            set => this.header[13] = (byte)((this.header[13] & Jacdac.Constants.JD_SERVICE_INDEX_INV_MASK) | value);
        }

        public ushort Crc => BitConverter.ToUInt16(this.header, 0);

        public ushort ServiceCommand
        {
            get => BitConverter.ToUInt16(this.header, 14);
            set => Util.Write16(this.header, 14, value);
        }

        public bool IsEvent => this.IsReport && this.ServiceIndex <= 0x30 && ((this.ServiceCommand & Jacdac.Constants.CMD_EVENT_MASK) != 0);

        public ushort EventCode
        {
            get
            {
                if (this.IsEvent)
                    return (ushort)(this.ServiceCommand & Jacdac.Constants.CMD_EVENT_CODE_MASK);
                return 0xffff;
            }
        }

        public uint EventCounter
        {
            get
            {
                if (this.IsEvent)
                    return ((uint)this.ServiceCommand >> Jacdac.Constants.CMD_EVENT_COUNTER_POS) & Jacdac.Constants.CMD_EVENT_COUNTER_MASK;
                return 0;
            }
        }

        public ushort RegisterCode => (ushort)(this.ServiceCommand & Jacdac.Constants.CMD_REG_MASK);
        public bool IsRegisterSet => this.ServiceIndex <= Jacdac.Constants.JD_SERVICE_INDEX_MAX_NORMAL && (this.ServiceCommand >> 12) == (Jacdac.Constants.CMD_SET_REG >> 12);
        public bool IsRegisterGet => this.ServiceIndex <= Jacdac.Constants.JD_SERVICE_INDEX_MAX_NORMAL && (this.ServiceCommand >> 12) == (Jacdac.Constants.CMD_GET_REG >> 12);

        public byte[] Header
        {
            get => this.header;
        }
        public byte[] Data
        {
            get => this.data;
        }

        public bool IsCommand
        {
            get => (this.FrameFlags & Jacdac.Constants.JD_FRAME_FLAG_COMMAND) != 0;
            set
            {
                unchecked
                {
                    if (value) this.header[3] |= (byte)Jacdac.Constants.JD_FRAME_FLAG_COMMAND;
                    else this.header[3] &= (byte)~Jacdac.Constants.JD_FRAME_FLAG_COMMAND;
                }
            }

        }
        public bool IsReport => !this.IsCommand;
    }

    public sealed class PacketEventArgs : EventArgs
    {
        public readonly Packet Packet;
        internal PacketEventArgs(Packet packet)
        {
            this.Packet = packet;
        }
    }
    public delegate void PacketEventHandler(JDNode sensor, PacketEventArgs e);
}
