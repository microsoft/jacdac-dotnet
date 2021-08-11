using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacdac
{
    enum NumberFormat
    {
        Int8LE = 1,
        UInt8LE = 2,
        Int16LE = 3,
        UInt16LE = 4,
        Int32LE = 5,
        Int8BE = 6,
        UInt8BE = 7,
        Int16BE = 8,
        UInt16BE = 9,
        Int32BE = 10,
        UInt32LE = 11,
        UInt32BE = 12,
        Float32LE = 13,
        Float64LE = 14,
        Float32BE = 15,
        Float64BE = 16,
        UInt64LE = 17,
        UInt64BE = 18,
        Int64LE = 19,
        Int64BE = 20,
        None
    }

    public struct ParseResult
    {
        public bool IsSingleValue => Values.Length == 1;

        public dynamic[] Values { get; private set; }

        public dynamic Value => Values[0];

        public T GetValue<T>(int idx = 0)
        {
            if (idx > Values.Length)
                throw new ArgumentOutOfRangeException();

            return (T)Values[idx];
        }

        public ParseResult(dynamic[] values)
        {
            Values = values;
        }
    }

    internal static class RegisterParser
    {
        private static byte GetNumberSize(NumberFormat format)
        {
            switch (format)
            {
                case NumberFormat.Int8LE:
                case NumberFormat.UInt8LE:
                case NumberFormat.Int8BE:
                case NumberFormat.UInt8BE:
                    return 1;
                case NumberFormat.Int16LE:
                case NumberFormat.UInt16LE:
                case NumberFormat.Int16BE:
                case NumberFormat.UInt16BE:
                    return 2;
                case NumberFormat.Int32LE:
                case NumberFormat.Int32BE:
                case NumberFormat.UInt32BE:
                case NumberFormat.UInt32LE:
                case NumberFormat.Float32BE:
                case NumberFormat.Float32LE:
                    return 4;
                case NumberFormat.UInt64BE:
                case NumberFormat.Int64BE:
                case NumberFormat.UInt64LE:
                case NumberFormat.Int64LE:
                case NumberFormat.Float64BE:
                case NumberFormat.Float64LE:
                    return 8;
                default:
                    return 0;
            }
        }

        private static NumberFormat GetNumberFormatOfType(string tp)
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
                    return NumberFormat.None;
            }
        }

        public static dynamic ParseNumber(NumberFormat format, ReadOnlySpan<byte> buffer) => format switch
        {
            NumberFormat.UInt8LE => buffer[0],
            NumberFormat.Int8LE => buffer[0],
            NumberFormat.UInt16LE => BitConverter.ToUInt16(buffer),
            NumberFormat.Int16LE => BitConverter.ToInt16(buffer),
            NumberFormat.UInt32LE => BitConverter.ToUInt32(buffer),
            NumberFormat.Int32LE => BitConverter.ToInt32(buffer),
            NumberFormat.UInt64LE => BitConverter.ToUInt64(buffer),
            NumberFormat.Int64LE => BitConverter.ToInt64(buffer),
            _ => 0
        };

        public static dynamic CastNumber(NumberFormat format, dynamic number) => format switch
        {
            NumberFormat.UInt8LE => (byte)number,
            NumberFormat.Int8LE => (byte)number,
            NumberFormat.UInt16LE => (ushort)number,
            NumberFormat.Int16LE => (short)number,
            NumberFormat.UInt32LE => (UInt32) number,
            NumberFormat.Int32LE => (Int32)number,
            NumberFormat.UInt64LE => (UInt64)number,
            NumberFormat.Int64LE => (Int64)number,
            _ => 0
        };

        private enum TokenType
        {
            None,
            Number,
            Buffer,
            String,
            TerminatedString,
            Repeat,
            Padding
        }

        private static TokenType GetTokenType(char chr) => chr switch
        {
            'u' => TokenType.Number,
            'i' => TokenType.Number,
            'b' => TokenType.Buffer,
            's' => TokenType.String,
            'z' => TokenType.TerminatedString,
            'x' => TokenType.Padding,
            'r' => TokenType.Repeat,
            _ => TokenType.None
        };

        private struct TokenResult
        {
            public TokenType Type { get; set; }

            public NumberFormat NumberFormat { get; set; }

            public byte Count { get; set; }

            public bool IsSigned { get; set; }

            public int Divisor { get; set; }

            public bool ExhaustBuffer { get; set; }

            public bool IsArray { get; set; }
        }

        private static TokenResult[] ParseFormat(string fmt)
        {
            var tokenStrs = fmt.Split(' ');
            var tokens = new TokenResult[tokenStrs.Length];

            for (int i = 0; i < tokenStrs.Length; i++)
            {
                var fpr = 0;

                var tokenRes = new TokenResult();

                var str = tokenStrs[i];
                tokenRes.Type = GetTokenType(str[fpr]);
                tokenRes.IsSigned = str[fpr] == 'i';
                tokenRes.Count = 1;

                fpr++;

                // If we have a number token, figure out whether it is a float
                if (tokenRes.Type == TokenType.Number && fpr < str.Length)
                {
                    var endIdx = str.Length;
                    var brIdx = str.IndexOf('[');
                    var readUntil = brIdx > 0 ? brIdx : endIdx;

                    var ptIdx = str.IndexOf('.');
                    if (ptIdx > 0)
                    {
                        var sz1 = byte.Parse(str.Substring(fpr, ptIdx - fpr));
                        var sz2 = byte.Parse(str.Substring(ptIdx + 1, readUntil - ptIdx - 1));
                        tokenRes.Divisor = 1 << sz2;
                        tokenRes.NumberFormat = GetNumberFormatOfType(str[0] + (sz1 + sz2).ToString());
                    }
                    else
                    {
                        tokenRes.NumberFormat = GetNumberFormatOfType(str.Substring(0, readUntil));
                    }
                    fpr = readUntil;
                }

                // Figure out whether we have an (unbounded) array
                if (fpr < str.Length && str[fpr] == '[')
                {
                    tokenRes.IsArray = true;

                    if (str[fpr + 1] == ']')
                        tokenRes.ExhaustBuffer = true;
                    else
                        tokenRes.Count = byte.Parse(str.Substring(fpr + 1, str.Length - fpr - 2));

                }
                else if (tokenRes.Type == TokenType.Buffer || tokenRes.Type == TokenType.String)
                    tokenRes.ExhaustBuffer = true;

                tokens[i] = tokenRes;
            }

            return tokens;
        }

        public static ParseResult ParseBuffer(string fmt, byte[] buffer)
        {
            var trm = fmt.Trim();

            if (trm == "b")
                return new ParseResult(new dynamic[] { buffer });

            var numberFormat = GetNumberFormatOfType(fmt);
            ReadOnlySpan<byte> spn = buffer;
            if (numberFormat != NumberFormat.None)
            {
                var size = GetNumberSize(numberFormat);
                if (buffer.Length < size)
                    throw new ArgumentOutOfRangeException($"Buffer too small. Expected size {size}, got {buffer.Length}");

                return new ParseResult(new dynamic[] { ParseNumber(numberFormat, spn) });
            }

            var results = new List<dynamic>();
            var tokens = ParseFormat(fmt);

            var repeatIndex = -1;
            var bufSlice = spn;
            for (int i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];

                if (token.Type == TokenType.Repeat)
                {
                    repeatIndex = i;
                    continue;
                }
                else if (token.IsArray && token.ExhaustBuffer)
                    repeatIndex = i - 1;

                var element = ParseElement(token, bufSlice, out var consumed);
                results.Add(element);
                bufSlice = bufSlice.Slice(consumed);

                if (bufSlice.Length == 0)
                    break;

                if (repeatIndex >= 0 && i == tokens.Length - 1)
                    i = repeatIndex;
            }

            return new ParseResult(results.ToArray());
        }

        private static dynamic ParseElement(TokenResult token, ReadOnlySpan<byte> buffer, out int consumed)
        {
            var length = token.ExhaustBuffer ? buffer.Length : token.Count;
            consumed = length;

            if (token.Type == TokenType.Number)
            {
                dynamic[] entries = new dynamic[token.Count];
                int bfStart = 0;
                var numberSize = GetNumberSize(token.NumberFormat);
                for (int i = 0; i < entries.Length; i++)
                {

                    dynamic number = ParseNumber(token.NumberFormat, buffer.Slice(bfStart, numberSize));
                    bfStart += numberSize;

                    if (token.IsSigned)
                    {
                        if (token.Divisor > 0)
                            entries[i] = (long)number / (float)token.Divisor;
                       else 
                            entries[i] = CastNumber(token.NumberFormat, number);
                    }
                    else
                    {
                        if (token.Divisor > 0)
                            entries[i] = (ulong)number / (float)token.Divisor;
                        else
                            entries[i] = CastNumber(token.NumberFormat, number);
                    }
                }

                consumed = numberSize * token.Count;

                if (entries.Length == 1)
                    return entries[0];

                return entries;
            }
            else if(token.Type == TokenType.TerminatedString)
            {
                length = buffer.IndexOf((byte)0);
                if (length < 0)
                    length = buffer.Length;

                consumed = length + 1;

                return Encoding.UTF8.GetString(buffer.Slice(0, length));
            }
            else if(token.Type == TokenType.String)
            {
                return Encoding.UTF8.GetString(buffer.Slice(0, length));
            }
            else if(token.Type == TokenType.Buffer)
            {
                return buffer.Slice(0, length).ToArray();
            } else if(token.Type == TokenType.Padding)
            {
                return null;
            }

            consumed = 0;
            return null;
        }
    }

}