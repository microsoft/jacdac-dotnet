using System;
using System.Collections.Generic;

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

    public sealed class Pack
    {
        const char ch_0 = (char)0;
        const int ch_b = 98;
        const int ch_i = 105;
        const int ch_r = 114;
        const int ch_s = 115;
        const int ch_u = 117;
        const int ch_x = 120;
        const int ch_z = 122;
        //const  ch_0 = 48
        //const  ch_9 = 57
        const int ch_colon = 58;
        const int ch_sq_open = 91;
        const int ch_sq_close = 93;

        internal sealed class TokenParser
        {
            public readonly string fmt;
            public int c0;
            public int size;
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
                    while (endp < fmt.Length & fmt[endp] != 32) endp++;
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

                    var c1 = word[1];
                    if (c1 == ch_sq_open)
                    {
                        this.size = int.Parse(word.Slice(2, word.Length));
                    }
                    else
                    {
                        this.size = -1;
                    }

                    if (
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
                            if (word.Length != 1 && this.size == -1) c0 = ch_0;
                        }
                        else if (c0 == ch_z)
                        {
                            if (word.Length != 1) c0 = ch_0;
                        }
                        else
                        {
                            c0 = ch_0;
                        }
                        if (c0 == 0) throw new ArgumentException("invalid format");
                        this.c0 = c0;
                    }
                    else
                    {
                        this.size = Util.SizeOfNumberFormat(this.nfmt);
                        this.c0 = -1;
                    }

                    return true;
                }
                return false;
            }
        }

        static object[] jdunpackCore(byte[] buf, string fmt, int repeat)
        {
            List<Object[]> repeatRes = repeat > 0 ? new List<Object[]>() : null;
            var res = new System.Collections.ArrayList();
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
                var sz = parser.size;
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
                    float v = Util.GetNumber(buf, parser.nfmt, off);
                    if (parser.div != 1) v /= parser.div;
                    off += parser.size;
                }
                else
                {
                    var subbuf = Util.Slice(buf, off, off + sz);
                    if (c0 == ch_z || c0 == ch_s)
                    {
                        var zerop = 0;
                        while (zerop < subbuf.Length && subbuf[zerop] != 0) zerop++;
                        res.Add(System.Text.UTF8Encoding.UTF8.GetString(Util.Slice(subbuf, 0, zerop)));
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

        /**
         Unpacks a byte buffer into structured data as specified in the format string.
         See jdpack for format string reference.
         @category Data Packing
*/
        public static object[] jdunpack(byte[] buf, string fmt)
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
    }
}
