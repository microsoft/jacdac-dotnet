using System;
using System.Collections;
using System.IO;

namespace Jacdac
{
    internal static class Extensions
    {
        /// <summary>
        /// Get the string slice between the two indexes.
        /// Inclusive for start index, exclusive for end index.
        /// </summary>
        public static string Slice(this string source, int start, int end)
        {
            if (end <= 0) // Keep this for negative end support
            {
                end = source.Length + end;
            }
            int len = end - start;               // Calculate length
            return source.Substring(start, len); // Return Substring of length
        }
    }

    public static class PacketEncoding
    {
        const char ch_0 = (char)0;
        const int ch_b = 98;
        const int ch_i = 105;
        const int ch_r = 114;
        const int ch_s = 115;
        const int ch_u = 117;
        const int ch_x = 120;
        const int ch_z = 122;
        const int ch_space = 32;
        //const  ch_0 = 48
        //const  ch_9 = 57
        const int ch_colon = 58;
        const int ch_sq_open = 91;
        const int ch_sq_close = 93;

        internal sealed class TokenParser
        {
            public readonly string fmt;
            public int c0;
            public int Size;
            public int div;
            public int fp = 0;
            public NumberFormat nfmt;
            public string word;
            public bool isArray;

            public TokenParser(string fmt)
            {
                this.fmt = fmt;
            }

            public bool Parse()
            {
                this.div = 1;
                this.isArray = false;

                var fmt = this.fmt;
                while (this.fp < fmt.Length)
                {
                    var endp = this.fp;
                    while (endp < fmt.Length && fmt[endp] != ch_space) endp++;
                    var word = fmt.Slice(this.fp, endp);
                    this.fp = endp + 1;
                    if (word.Length == 0) continue;

                    var dotIdx = word.IndexOf(".");
                    var c0 = word[0];
                    // "u10.6" -> "u16", div = 1 << 6
                    if ((c0 == ch_i || c0 == ch_u) && dotIdx >= 0)
                    {
                        var sz0 = int.Parse(word.Slice(1, dotIdx));
                        var sz1 = int.Parse(word.Slice(dotIdx + 1, word.Length));
                        word = word[0] + (sz0 + sz1).ToString();
                        this.div = 1 << sz1;
                    }

                    var c1 = word.Length > 1 ? word[1] : ch_0;
                    if (c1 == ch_sq_open)
                    {
                        this.Size = int.Parse(word.Slice(2, word.Length - 1));
                    }
                    else
                    {
                        this.Size = -1;
                    }

                    if (
                        word.Length > 2 &&
                        word[word.Length - 1] == ch_sq_close &&
                        word[word.Length - 2] == ch_sq_open
                    )
                    {
                        word = word.Slice(0, -2);
                        this.isArray = true;
                    }

                    this.nfmt = Util.NumberFormatOfType(word);
                    this.word = word;

                    if (this.nfmt == NumberFormat.Unknown)
                    {
                        if (c0 == ch_r)
                        {
                            if (c1 != ch_colon) c0 = ch_0;
                        }
                        else if (c0 == ch_s || c0 == ch_b || c0 == ch_x)
                        {
                            if (word.Length != 1 && this.Size == -1) c0 = ch_0;
                        }
                        else if (c0 == ch_z)
                        {
                            if (word.Length != 1) c0 = ch_0;
                        }
                        else
                        {
                            c0 = ch_0;
                        }
                        if (c0 == ch_0) throw new ArgumentException("invalid format");
                        this.c0 = c0;
                    }
                    else
                    {
                        this.Size = Util.SizeOfNumberFormat(this.nfmt);
                        this.c0 = -1;
                    }

                    return true;
                }
                return false;
            }
        }

        private static object[] jdunpackCore(byte[] buf, string fmt, int repeat)
        {
            ArrayList repeatRes = repeat > 0 ? new ArrayList() : null;
            var res = new ArrayList();
            var off = 0;
            var fp0 = 0;
            var parser = new TokenParser(fmt);
            if (repeat > 0 && buf.Length == 0) return new object[][] { };
            while (parser.Parse())
            {
                if (parser.isArray && repeat == 0)
                {
                    res.Add(
                        jdunpackCore(
                            Util.Slice(buf, off, buf.Length),
                            fmt.Slice(fp0, fmt.Length),
                            1
                        )
                    );
                    return res.ToArray();
                }

                fp0 = parser.fp;
                var sz = parser.Size;
                var c0 = parser.c0;
                if (c0 == ch_z)
                {
                    var endoff = off;
                    while (endoff < buf.Length && buf[endoff] != 0) endoff++;
                    sz = endoff - off;
                }
                else if (sz < 0)
                {
                    sz = buf.Length - off;
                }

                if (parser.nfmt != NumberFormat.Unknown)
                {
                    object v = Util.GetNumber(buf, parser.nfmt, off);
                    switch (parser.nfmt)
                    {
                        case NumberFormat.Int8LE:
                            v = (int)v;
                            if (parser.div != 1) v = (float)((double)(int)v / parser.div);
                            break;
                        case NumberFormat.UInt8LE:
                            v = (uint)v;
                            if (parser.div != 1) v = (float)((double)(uint)v / parser.div);
                            break;
                        case NumberFormat.Int16LE:
                            v = (int)v;
                            if (parser.div != 1) v = (float)((double)(int)v / parser.div);
                            break;
                        case NumberFormat.UInt16LE:
                            v = (uint)v;
                            if (parser.div != 1) v = (float)((double)(uint)v / parser.div);
                            break;
                        case NumberFormat.Int32LE:
                            v = (int)v;
                            if (parser.div != 1) v = (float)((double)(int)v / parser.div);
                            break;
                        case NumberFormat.UInt32LE:
                            v = (uint)v;
                            if (parser.div != 1) v = (float)((double)(uint)v / parser.div);
                            break;
                        case NumberFormat.Int64LE:
                            v = (long)v;
                            if (parser.div != 1) v = (float)((double)(long)v / parser.div);
                            break;
                        case NumberFormat.UInt64LE:
                            v = (ulong)v;
                            if (parser.div != 1) v = (float)((double)(ulong)v / parser.div);
                            break;
                        case NumberFormat.Float32LE:
                            v = (float)v;
                            if (parser.div != 1) v = (float)((float)v / parser.div);
                            break;
                        case NumberFormat.Float64LE:
                            v = (double)v;
                            if (parser.div != 1) v = (float)((double)v / parser.div);
                            break;
                        default:
                            throw new ArgumentException("unknown data format");
                    }
                    res.Add(v);
                    off += parser.Size;
                }
                else
                {
                    var subbuf = Util.Slice(buf, off, off + sz);
                    if (c0 == ch_z || c0 == ch_s)
                    {
                        var zerop = 0;
                        while (zerop < subbuf.Length && subbuf[zerop] != 0) zerop++;
                        res.Add(System.Text.UTF8Encoding.UTF8.GetString(subbuf, 0, zerop));
                    }
                    else if (c0 == ch_b)
                    {
                        res.Add(subbuf);
                    }
                    else if (c0 == ch_x)
                    {
                        // skip padding
                    }
                    else if (c0 == ch_r)
                    {
                        res.Add(jdunpackCore(subbuf, fmt.Slice(fp0, fmt.Length), 2));
                        break;
                    }
                    else
                    {
                        throw new InvalidOperationException("Unreachable case");
                    }
                    off += subbuf.Length;
                    if (c0 == ch_z) off++;
                }

                if (repeat > 0 && parser.fp >= fmt.Length)
                {
                    parser.fp = 0;
                    if (repeat == 2)
                    {
                        repeatRes.Add(res.ToArray() as object[]);
                        res = new System.Collections.ArrayList();
                    }
                    if (off >= buf.Length) break;
                }
            }

            if (repeat == 2)
            {
                if (res.Count > 0) repeatRes.Add(res.ToArray());
                return repeatRes.ToArray();
            }
            else
            {
                return res.ToArray();
            }
        }

        /// <summary>
        /// Unpacks a byte buffer into structured data as specified in the format string.
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="fmt"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>        
        public static object[] UnPack(string fmt, byte[] buf)
        {
            if (buf == null || String.IsNullOrEmpty(fmt)) return null;

            // hot path for buffers
            if (fmt == "b") return new object[] { buf.Clone() };
            // hot path
            var nf = Util.NumberFormatOfType(fmt);
            if (nf != NumberFormat.Unknown)
            {
                var sz = Util.SizeOfNumberFormat(nf);
                if (buf.Length < sz)
                    throw new ArgumentOutOfRangeException("size mistmatch");
                return new object[] { Util.GetNumber(buf, nf, 0) };
            }
            // slow path
            return jdunpackCore(buf, fmt, 0);
        }

        static int jdpackCore(byte[] trg, string fmt, object[] data, int off)
        {
            //console.log({ fmt, data })
            var idx = 0;
            var parser = new TokenParser(fmt);
            while (parser.Parse())
            {
                var c0 = parser.c0;

                if (c0 == ch_x)
                {
                    // skip padding
                    off += parser.Size;
                    continue;
                }

                var dataItem = idx < data.Length ? data[idx++] : null;
                if (c0 == ch_r && dataItem != null)
                {
                    var fmt0 = fmt.Substring(parser.fp);
                    foreach (var velt in dataItem as object[])
                    {
                        off = jdpackCore(trg, fmt0, velt as object[], off);
                    }
                    break;
                }

                // use temporary variable to avoid a Gatsby build bug
                object[] arr;
                if (parser.isArray) arr = (object[])dataItem;
                else arr = new object[] { dataItem };

                foreach (var v in arr)
                {
                    if (parser.nfmt != NumberFormat.Unknown)
                    {
                        if (!Util.IsNumber(v))
                            throw new ArgumentException($"expecting number, got {v.GetType()}");
                        if (trg != null)
                        {
                            Util.SetNumber(
                                trg,
                                off,
                                parser.nfmt,
                                v,
                                parser.div
                            );
                        }
                        off += parser.Size;
                    }
                    else
                    {
                        byte[] buf;
                        if (v is string)
                        {
                            var s = (string)v;
                            if (c0 == ch_z) buf = System.Text.UTF8Encoding.UTF8.GetBytes(s + "\u0000");
                            else if (c0 == ch_s) buf = System.Text.UTF8Encoding.UTF8.GetBytes(s);
                            else throw new ArgumentException("unexpected string");
                        }
                        else if (v != null && v is byte[])
                        {
                            // assume buffer
                            if (c0 == ch_b) buf = (byte[])v;
                            else throw new ArgumentException("unexpected buffer");
                        }
                        else
                        {
                            throw new ArgumentException("expecting string or buffer");
                        }

                        var sz = parser.Size;
                        if (sz >= 0)
                        {
                            if (buf.Length > sz) buf = Util.Slice(buf, 0, sz);
                        }
                        else
                        {
                            sz = buf.Length;
                        }

                        if (trg != null) buf.CopyTo(trg, off);
                        off += sz;
                    }
                }
            }

            if (data.Length > idx) throw new ArgumentException($"format '{fmt}' too short");

            return off;
        }


        /**

* Format strings are space-separated sequences of type descriptions.
* All numbers are understood to be little endian.
* The following type descriptions are supported:
* 
* - `u8`, `u16`, `u32` - unsigned, 1, 2, and 4 bytes long respectively
* - `i8`, `i16`, `i32` - similar, but signed
* - `b` - buffer until the end of input (has to be last)
* - `s` - similar, but utf-8 encoded string
* - `z` - NUL-terminated utf-8 string
* - `b[10]` - 10 byte buffer (10 is just an example, here and below)
* - `s[10]` - 10 byte utf-8 string; trailing NUL bytes (if any) are removed
* - `x[10]` - 10 bytes of padding
* 
* There is one more token, `r:`. The type descriptions following it are repeated in order
* until the input buffer is exhausted.
* When unpacking, fields after `r:` are repeated as an array of tuples.
* 
* In case there's only a single field repeating,
* it's also possible to append `[]` to its type, to get an array of values.
* 
* @category Data Packing
*/
        public static byte[] Pack(string fmt, object[] data)
        {
            if (String.IsNullOrEmpty(fmt) || data == null) return null;

            // hot path for buffers
            if (fmt == "b")
            {
                var b = (byte[])data[0];
                return b != null ? (byte[])b.Clone() : null;
            }
            // hot path, single value
            var nf = Util.NumberFormatOfType(fmt);
            if (nf != NumberFormat.Unknown)
            {
                byte[] buf = new byte[Util.SizeOfNumberFormat(nf)];
                Util.SetNumber(buf, 0, nf, data[0], 1);
                return buf;
            }
            // slow path
            var len = jdpackCore(null, fmt, data, 0);
            var res = new byte[len];
            jdpackCore(res, fmt, data, 0);
            return res;
        }
    }
}
