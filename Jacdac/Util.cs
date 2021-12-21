using System;
using System.Diagnostics;
using System.Text;

namespace Jacdac
{
    public static class HexEncoding
    {
        private const string HEX_CHARS = "0123456789abcdef";
        public static string ToString(byte[] data, int start, int length)
        {
            Stats.HexEncode++;
            var n = Math.Min(length, data.Length - start);
            var chars = new char[n * 2];
            for (var i = 0; i < n; i++)
            {
                var b = data[start + i];
                chars[i << 1] = HexEncoding.HEX_CHARS[(b >> 4) & 0xf];
                chars[(i << 1) + 1] = HexEncoding.HEX_CHARS[b & 0xf];
            }
            var res = new String(chars);
            return res;
        }

        public static string ToString(byte[] data)
        {
            return ToString(data, 0, data.Length);
        }

        public static byte[] ToBuffer(string hex)
        {
            Stats.HexDecode++;
            var data = new byte[hex.Length / 2];
            for (var i = 0; i < hex.Length; i += 2)
            {
                var sub = hex.Substring(i, 2);
                data[i >> 1] = (byte)Convert.ToInt32(sub, 16);
            }
            return data;
        }
    }

    internal enum NumberFormat
    {
        Unknown = 0,
        Int8LE = 1,
        UInt8LE = 2,
        Int16LE = 3,
        UInt16LE = 4,
        Int32LE = 5,
        //Int8BE = 6,
        //UInt8BE = 7,
        //Int16BE = 8,
        //UInt16BE = 9,
        //Int32BE = 10,
        UInt32LE = 11,
        //UInt32BE = 12,
        Float32LE = 13,
        Float64LE = 14,
        //Float32BE = 15,
        //Float64BE = 16,
        UInt64LE = 17,
        //UInt64BE = 18,
        Int64LE = 19,
        //Int64BE = 20,
    }
    internal static class Util
    {
        public static bool IsNumber(object value)
        {
            return value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double;
        }

        public static byte[] Slice(byte[] source, int start, int end = 0)
        {
            if (start < 0)
            {
                start = source.Length + start;
            }

            if (end == 0)
            {

                var dest = new byte[source.Length - start];

                Array.Copy(source, start, dest, 0, dest.Length);

                return dest;

            }
            else
            {
                if (end > 0)
                {
                    var length = end - start;

                    var dest = new byte[length];

                    Array.Copy(source, start, dest, 0, dest.Length);

                    return dest;
                }
                else
                {
                    end = source.Length + end;

                    var length = end - start;

                    var dest = new byte[length];

                    Array.Copy(source, start, dest, 0, dest.Length);

                    return dest;
                }
            }
        }

        public static byte[] BufferConcat(byte[] a, byte[] b)
        {
            var r = new byte[a.Length + b.Length];

            Array.Copy(a, 0, r, 0, a.Length);
            Array.Copy(b, 0, r, a.Length, b.Length);

            return r;
        }

        public static bool BufferEquals(byte[] a, byte[] b, int offset = 0)
        {
            if (a == b) return true;
            if (a == null || b == null || a.Length != b.Length) return false;
            for (var i = offset; i < a.Length; ++i)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        public static void Write16(byte[] data, uint pos, ushort v)
        {
            data[pos + 0] = (byte)((v >> 0) & 0xff);
            data[pos + 1] = (byte)((v >> 8) & 0xff);
        }

        public static void Write32(byte[] data, uint pos, uint v)
        {
            data[pos + 0] = (byte)((v >> 0) & 0xff);
            data[pos + 1] = (byte)((v >> 8) & 0xff);
            data[pos + 2] = (byte)((v >> 16) & 0xff);
            data[pos + 3] = (byte)((v >> 24) & 0xff);
        }

        public static object GetNumber(byte[] buf, NumberFormat fmt, int offset)
        {
            switch (fmt)
            {
                case NumberFormat.UInt8LE:
                    return (uint)buf[offset];
                case NumberFormat.Int8LE:
                    return (int)(sbyte)buf[offset];
                case NumberFormat.UInt16LE:
                    return (uint)BitConverter.ToUInt16(buf, offset);
                case NumberFormat.Int16LE:
                    return (int)BitConverter.ToInt16(buf, offset);
                case NumberFormat.UInt32LE:
                    return BitConverter.ToUInt32(buf, offset);
                case NumberFormat.Int32LE:
                    return BitConverter.ToInt32(buf, offset);
                case NumberFormat.UInt64LE:
                    return BitConverter.ToUInt64(buf, offset);
                case NumberFormat.Int64LE:
                    return BitConverter.ToInt64(buf, offset);
                case NumberFormat.Float32LE:
                    return BitConverter.ToSingle(buf, offset);
                case NumberFormat.Float64LE:
                    return BitConverter.ToDouble(buf, offset);
                default:
                    throw new Exception("unsupported fmt:" + fmt);
            }
        }

        public static uint UnboxUint(object value)
        {
            if (value is uint) return (uint)value;
            if (value is ushort) return (uint)(ushort)value;
            if (value is byte) return (uint)(byte)value;
            if (value is int)
            {
                var i = (int)value;
                if (i < 0) throw new ArgumentOutOfRangeException("value");
                return (uint)i;
            }
            if (value is short)
            {
                var i = (short)value;
                if (i < 0) throw new ArgumentOutOfRangeException("value");
                return (uint)i;
            }
            if (value is sbyte)
            {
                var i = (sbyte)value;
                if (i < 0) throw new ArgumentOutOfRangeException("value");
                return (uint)i;
            }
            if (value is bool)
            {
                return (bool)value ? 1u : 0u;
            }
            // try for enums
            return (uint)value;
        }
        public static int UnboxInt(object value)
        {
            if (value is int) return (int)value;
            if (value is short) return (int)(short)value;
            if (value is sbyte) return (int)(sbyte)value;
            if (value is ushort) return (int)(ushort)value;
            if (value is byte) return (int)(byte)value;
            if (value is uint)
            {
                var i = (uint)value;
                if (i > int.MaxValue) throw new ArgumentOutOfRangeException("value");
                return (int)i;
            }
            if (value is bool)
            {
                return (bool)value ? 1 : 0;
            }
            // try for enums
            return (int)value;
        }

        public static int SetNumber(
            byte[] buf,
            int offset,
            NumberFormat fmt,
            object r,
            int div
        )
        {
            byte[] bytes = null;
            switch (fmt)
            {
                case NumberFormat.UInt8LE:
                    {
                        byte v;
                        if (div != 1) v = (byte)((float)r * div);
                        else v = (byte)UnboxUint(r);
                        bytes = new byte[] { v };
                        break;
                    }
                case NumberFormat.Int8LE:
                    {
                        var v = (byte)UnboxInt(r);
                        if (div != 1)
                            v = (byte)((float)v * div);
                        bytes = new byte[] { (byte)v };
                        break;
                    }
                case NumberFormat.UInt16LE:
                    {
                        ushort v;
                        if (div != 1)
                        {
                            var f = (float)r * div;
                            if (f < ushort.MinValue) f = ushort.MinValue;
                            else if (f > ushort.MaxValue) f = ushort.MaxValue;
                            v = (ushort)f;
                        }
                        else v = (ushort)UnboxUint(r);
                        bytes = BitConverter.GetBytes(v);
                        break;
                    }
                case NumberFormat.Int16LE:
                    {
                        short v;
                        if (div != 1)
                        {
                            var f = (float)r * div;
                            if (f < short.MinValue) f = short.MinValue;
                            else if (f > short.MaxValue) f = short.MaxValue;
                            v = (short)f;
                        }
                        else v = (short)UnboxInt(r);
                        bytes = BitConverter.GetBytes(v);
                        break;
                    }
                case NumberFormat.UInt32LE:
                    {
                        uint v;
                        if (div != 1)
                        {
                            var f = (float)r * div;
                            if (f < uint.MinValue) f = uint.MinValue;
                            else if (f > uint.MaxValue) f = uint.MaxValue;
                            v = (uint)f;
                        }
                        else v = UnboxUint(r);
                        bytes = BitConverter.GetBytes(v);
                        break;
                    }
                case NumberFormat.Int32LE:
                    {
                        int v;
                        if (div != 1)
                        {
                            var f = (float)r * div;
                            if (f < int.MinValue) f = int.MinValue;
                            else if (f > int.MaxValue) f = int.MaxValue;
                            v = (int)f;
                        }
                        else v = UnboxInt(r);
                        bytes = BitConverter.GetBytes(v);
                        break;
                    }
                case NumberFormat.UInt64LE:
                    {
                        ulong v;
                        if (div != 1)
                        {
                            var f = (double)r * div;
                            if (f < ulong.MinValue) f = ulong.MinValue;
                            else if (f > ulong.MaxValue) f = ulong.MaxValue;
                            v = (ulong)f;
                        }
                        else v = (ulong)r;
                        bytes = BitConverter.GetBytes(v);
                        break;
                    }
                case NumberFormat.Int64LE:
                    {
                        long v;
                        if (div != 1)
                        {
                            var f = (double)r * div;
                            if (f < long.MinValue) f = long.MinValue;
                            else if (f > long.MaxValue) f = long.MaxValue;
                            v = (long)f;
                        }
                        else v = (long)r;
                        bytes = BitConverter.GetBytes(v);
                        break;
                    }
                case NumberFormat.Float32LE:
                    {
                        var v = (float)r;
                        if (div != 1)
                            v = (float)((float)v * div);
                        bytes = BitConverter.GetBytes(v);
                        break;
                    }
                case NumberFormat.Float64LE:
                    {
                        var v = (double)r;
                        if (div != 1)
                            v = (double)((float)v * div);
                        bytes = BitConverter.GetBytes(v);
                        break;
                    }
            }
            if (bytes == null)
                throw new ArgumentException("unsupported format " + fmt);
            bytes.CopyTo(buf, offset);
            return offset + bytes.Length;
        }

        public static int SizeOfNumberFormat(NumberFormat format)
        {
            switch (format)
            {
                case NumberFormat.Int8LE:
                case NumberFormat.UInt8LE:
                    return 1;
                case NumberFormat.Int16LE:
                case NumberFormat.UInt16LE:
                    return 2;
                case NumberFormat.Int32LE:
                case NumberFormat.UInt32LE:
                case NumberFormat.Float32LE:
                    return 4;
                case NumberFormat.UInt64LE:
                case NumberFormat.Int64LE:
                case NumberFormat.Float64LE:
                    return 8;
            }
            return 0;
        }
        public static int NumberFormatToStorageType(NumberFormat nf)
        {
            switch (nf)
            {
                case NumberFormat.Int8LE:
                    return -1;
                case NumberFormat.UInt8LE:
                    return 1;
                case NumberFormat.Int16LE:
                    return -2;
                case NumberFormat.UInt16LE:
                    return 2;
                case NumberFormat.Int32LE:
                    return -4;
                case NumberFormat.UInt32LE:
                    return 4;
                case NumberFormat.Int64LE:
                    return -8;
                case NumberFormat.UInt64LE:
                    return 8;
                default:
                    return 0;
            }
        }

        public static NumberFormat NumberFormatOfType(string tp)
        {
            switch (tp)
            {
                case "u8":
                    return NumberFormat.UInt8LE;
                case "u16":
                    return NumberFormat.UInt16LE;
                case "u32":
                    return NumberFormat.UInt32LE;
                case "i8":
                    return NumberFormat.Int8LE;
                case "i16":
                    return NumberFormat.Int16LE;
                case "i32":
                    return NumberFormat.Int32LE;
                case "f32":
                    return NumberFormat.Float32LE;
                case "f64":
                    return NumberFormat.Float64LE;
                case "i64":
                    return NumberFormat.Int64LE;
                case "u64":
                    return NumberFormat.UInt64LE;
                default:
                    return NumberFormat.Unknown;
            }
        }

        public static ushort CRC(byte[] p) => CRC(p, 0, p.Length);

        public static ushort CRC(byte[] p, int start, int size) => Platform.Crc16(p, start, size);

        static uint Fnv1(byte[] data)
        {
            var h = 0x811c9dc5;
            for (var i = 0; i < data.Length; ++i)
            {
                h = (h * 0x1000193) ^ data[i];
            }
            return h;
        }
        static uint Hash(byte[] buf, int bits)
        {
            if (bits < 1) return 0;
            var h = Fnv1(buf);
            if (bits >= 32) return h >> 0;
            else return (uint)((h ^ (h >> bits)) & ((1 << bits) - 1)) >> 0;
        }
        public static string ShortDeviceId(string devid)
        {
            var h = Hash(HexEncoding.ToBuffer(devid), 30);
            return new String(new char[] {
                (char)(0x41 + (h % 26)) ,
                (char)(0x41 + ((h / 26) % 26)) ,
                (char)(0x30 + ((h / (26 * 26)) % 10)) ,
                (char)(0x30 + ((h / (26 * 26 * 10)) % 10))
                });
        }
    }
}
