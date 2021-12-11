using System;
using System.Diagnostics;
using System.Collections;

namespace Jacdac
{
    [DebuggerDisplay("{DeviceId}[{ServiceIndex}]>{ServiceCommand}")]
    public sealed class Packet
    {
        byte[] header;
        byte[] data;
        public TimeSpan Timestamp { get; set; }

        public static readonly Packet[] EmptyFrame = new Packet[0];

        private Packet()
        {

        }

        public override string ToString()
        {
            return HexEncoding.ToString(this.ToBuffer());
        }

        public static Packet FromBinary(byte[] buffer)
        {
            var p = new Packet
            {
                header = Util.Slice(buffer, 0, (int)Constants.JD_SERIAL_HEADER_SIZE),
                data = Util.Slice(buffer, (int)Constants.JD_SERIAL_HEADER_SIZE)
            };

            return p;

        }

        public static Packet[] FromFrame(byte[] frame)
        {
            var size = frame.Length < 12 ? 0 : frame[2];
            if (frame.Length < size + 12)
            {
                Debug.WriteLine($"got only {frame.Length} bytes; expecting {size + 12}`");
                return Packet.EmptyFrame;

            }
            else if (size < 4)
            {
                Debug.WriteLine("empty packet");
                return Packet.EmptyFrame;
            }
            else
            {
                var computed = Util.CRC(Util.Slice(frame, 2, size + 12));
                var actual = Util.Read16(frame, 0);
                if (actual != computed)
                {
                    Debug.WriteLine($"crc mismatch; sz={size} got:{actual}, exp:{computed}");
                    return Packet.EmptyFrame;
                }

                ArrayList res = new ArrayList();
                if (frame.Length != 12 + size)
                {
                    Debug.WriteLine($"unexpected packet len: ${frame.Length}");
                    return Packet.EmptyFrame;
                }

                for (var ptr = 12; ptr < 12 + size;)
                {
                    var psz = frame[ptr] + 4;
                    var sz = (psz + 3) & ~3; // align
                    var pkt = Util.BufferConcat(
                        Util.Slice(frame, 0, 12),
                        Util.Slice(frame, ptr, ptr + psz)
                    );
                    if (ptr + psz > 12 + size)
                    {
                        Debug.WriteLine($"invalid frame compression, res len ={res.Count}");
                        break;
                    }
                    var p = Packet.FromBinary(pkt);
                    res.Add(p);
                    // only set req_ack flag on first packet - otherwise we would sent multiple acks
                    if (res.Count > 1) p.RequiresAck = false;
                    ptr += sz;
                }

                return (Packet[])res.ToArray(typeof(Packet));
            }
        }

        public static Packet From(ushort serviceCommand, byte[] buffer)
        {
            var p = new Packet
            {
                header = new byte[Jacdac.Constants.JD_SERIAL_HEADER_SIZE],
                data = buffer,
                ServiceCommand = serviceCommand
            };

            return p;
        }

        public static Packet OnlyHeader(ushort serviceCommand)
        {
            var data = new byte[0];
            return Packet.From(serviceCommand, data);
        }

        public byte[] ToBuffer() => Util.BufferConcat(this.header, this.data);

        public string DeviceId
        {
            get => HexEncoding.ToString(Util.Slice(this.header, 4, 4 + 8));
            set
            {
                var idb = HexEncoding.ToBuffer(value);
                if (idb.Length != 8)
                    throw new Exception("Invalid id");
                Array.Copy(idb, 0, this.header, 4, idb.Length);
            }
        }



        public byte FrameFlags => this.header[3];


        public bool IsMultiCommand
        {
            get { return (this.FrameFlags & Jacdac.Constants.JD_FRAME_FLAG_IDENTIFIER_IS_SERVICE_CLASS) != 0; }
        }

        public uint MulticommandClass
        {
            get
            {
                if ((this.FrameFlags & Jacdac.Constants.JD_FRAME_FLAG_IDENTIFIER_IS_SERVICE_CLASS) != 0)
                    return Util.Read32(this.header, 4);
                return Constants.UNDEFINED;
            }
        }

        public uint Size => this.header[12];

        public bool RequiresAck
        {
            get => (this.FrameFlags & Jacdac.Constants.JD_FRAME_FLAG_ACK_REQUESTED) != 0 ? true : false;

            set
            {
                if (value != this.RequiresAck)
                    this.header[3] ^= Jacdac.Constants.JD_FRAME_FLAG_ACK_REQUESTED;
            }
        }


        public byte ServiceIndex
        {
            get => (byte)(this.header[13] & Jacdac.Constants.JD_SERVICE_INDEX_MASK);
            set => this.header[13] = (byte)((this.header[13] & Jacdac.Constants.JD_SERVICE_INDEX_INV_MASK) | value);
        }

        public ushort Crc => Util.Read16(this.header, 0);

        public ushort ServiceCommand
        {
            get => Util.Read16(this.header, 14);
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
            set
            {
                if (value.Length > Jacdac.Constants.JD_SERIAL_HEADER_SIZE)
                    throw new ArgumentOutOfRangeException("Data payload too large");
                this.header = value;
            }
        }
        public byte[] Data
        {
            get => this.data;
            set
            {
                if (value.Length > Jacdac.Constants.JD_SERIAL_MAX_PAYLOAD_SIZE)
                    throw new ArgumentOutOfRangeException("Data payload too large");
                this.header[12] = (byte)value.Length;
                this.data = value;
            }
        }

        public bool IsCommand => (this.FrameFlags & Jacdac.Constants.JD_FRAME_FLAG_COMMAND) != 0;
        public bool IsReport => !this.IsCommand;

        public object[] UnPack(string format)
        {
            return PacketEncoding.UnPack(format, this.Data);
        }
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
