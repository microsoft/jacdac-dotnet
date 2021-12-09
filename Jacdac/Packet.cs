using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace Jacdac {
    public class Packet {
        byte[] header;
        byte[] data;
        public TimeSpan Timestamp { get; set; }
        Device dev;

        public Packet() {

        }

        public static Packet FromBinary(byte[] buffer) {
            var p = new Packet {
                header = Util.Slice(buffer, 0, Jacdac.Constants.JD_SERIAL_HEADER_SIZE),
                data = Util.Slice(buffer, Jacdac.Constants.JD_SERIAL_HEADER_SIZE)
            };

            return p;

        }

        public static Packet From(uint service_command, byte[] buffer) {
            var p = new Packet {
                header = new byte[Jacdac.Constants.JD_SERIAL_HEADER_SIZE],
                data = buffer,
                ServiceCommand = (ushort)service_command
            };

            return p;
        }

        public static Packet OnlyHeader(uint service_command) {
            var data = new byte[0];
            return Packet.From(service_command, data);
        }

        public byte[] ToBuffer() => Util.BufferConcat(this.header, this.data);

        public string DeviceIdentifier {
            get => Util.ToHex(Util.Slice(this.header, 4, 4 + 8));
            set {
                var idb = Util.FromHex(value);

                if (idb.Length != 8) {
                    throw new Exception("Invalid id");
                }

                Util.Set(this.header, idb, 4);
            }
        }



        public byte FrameFlags => this.header[3];


        public uint? MulticommandClass {
            get {
                if ((this.FrameFlags & Jacdac.Constants.JD_FRAME_FLAG_IDENTIFIER_IS_SERVICE_CLASS) != 0)
                    return Util.Read32(this.header, 4);
                return null;
            }
        }

        public uint Size => this.header[12];


        public bool IsRequiresAck {
            get => (this.FrameFlags & Jacdac.Constants.JD_FRAME_FLAG_ACK_REQUESTED) != 0 ? true : false;

            set {
                if (value != this.IsRequiresAck)
                    this.header[3] ^= Jacdac.Constants.JD_FRAME_FLAG_ACK_REQUESTED;
            }
        }


        public uint ServiceNumber {
            get => (uint)(this.header[13] & Jacdac.Constants.JD_SERVICE_NUMBER_MASK);
            set => this.header[13] = (byte)((this.header[13] & Jacdac.Constants.JD_SERVICE_NUMBER_INV_MASK) | value);
        }


        public uint? ServiceClass {
            get {
                if (this.dev != null)
                    return this.dev.ServiceAt(this.ServiceNumber);
                return null;
            }
        }
        public ushort Crc => Util.Read16(this.header, 0);


        public ushort ServiceCommand {
            get => Util.Read16(this.header, 14);
            set => Util.Write16(this.header, 14, value);
        }

        public bool IsEvent => this.IsReport && this.ServiceNumber <= 0x30 && ((this.ServiceCommand & Jacdac.Constants.CMD_EVENT_MASK) != 0);

        public uint? EventCode {
            get {
                if (this.IsEvent == true)
                    return this.ServiceCommand & Jacdac.Constants.CMD_EVENT_CODE_MASK;
                else
                    return null;
            }
        }

        public uint? EventCounter {
            get {
                if (this.IsEvent == true)
                    return ((uint)this.ServiceCommand >> Jacdac.Constants.CMD_EVENT_COUNTER_POS) & Jacdac.Constants.CMD_EVENT_COUNTER_MASK;
                else
                    return null;
            }
        }

        public uint RegisterCode => (uint)(this.ServiceCommand & Jacdac.Constants.CMD_REG_MASK);
        public bool IsRegisterSet => (this.ServiceCommand >> 12) == (Jacdac.Constants.CMD_SET_REG >> 12) ? true : false;
        public bool IsRegisterGet => (this.ServiceCommand >> 12) == (Jacdac.Constants.CMD_GET_REG >> 12) ? true : false;

        public byte[] Header {
            get => this.header;
            set {
                if (value.Length > Jacdac.Constants.JD_SERIAL_HEADER_SIZE)
                    throw new Exception("Too big");

                this.header = value;
            }
        }
        public byte[] Data {
            get => this.data;
            set {
                if (value.Length > Jacdac.Constants.JD_SERIAL_MAX_PAYLOAD_SIZE)
                    throw new Exception("Too big");
                this.header[12] = (byte)value.Length;
                this.data = value;
            }
        }

        public uint? UintData {
            get {
                var buf = this.data;

                if (buf == null || buf.Length == 0)
                    return null;

                if (buf.Length < 4)
                    buf = Util.BufferConcat(buf, new byte[4]);

                return Util.Read32(buf, 0);
            }
        }

        public int? IntData {
            get {
                Util.NumberFormat fmt;
                switch (this.data.Length) {
                    case 0:
                        return null;
                    case 1:
                        fmt = Util.NumberFormat.Int8LE;
                        break;
                    case 2:
                    case 3:
                        fmt = Util.NumberFormat.Int16LE;
                        break;
                    default:
                        fmt = Util.NumberFormat.Int32LE;
                        break;
                }
                return (int)this.GetNumber(fmt, 0);
            }
        }

        public uint GetNumber(Util.NumberFormat fmt, int offset) => Util.GetNumber(this.data, fmt, offset);

        public bool IsCommand => (this.FrameFlags & Jacdac.Constants.JD_FRAME_FLAG_COMMAND) != 0 ? true : false;
        public bool IsReport => !this.IsCommand;
    }
}
