using System;
using System.Collections;

namespace Jacdac
{
    /// <summary>
    /// Encode LED light instructions
    /// </summary>
    public sealed class LedPixelEncoder
    {
        /*
         * `0xD0: set_all(C+)` - set all pixels in current range to given color pattern
         * `0xD1: fade(C+)` - set `N` pixels to color between colors in sequence
         * `0xD2: fade_hsv(C+)` - similar to `fade()`, but colors are specified and faded in HSV
         * `0xD3: rotate_fwd(K)` - rotate (shift) pixels by `K` positions away from the connector
         * `0xD4: rotate_back(K)` - same, but towards the connector
         * `0xD5: show(M=50)` - send buffer to strip and wait `M` milliseconds
         * `0xD6: range(P=0, N=length)` - range from pixel `P`, `N` pixels long
         * `0xD7: mode(K=0)` - set update mode
         * `0xD8: mode1(K=0)` - set update mode for next command only
         */

        const byte LIGHT_PROG_SET_ALL = 0xd0;
        const byte LIGHT_PROG_FADE = 0xd1;
        const byte LIGHT_PROG_FADE_HSV = 0xd2;
        const byte LIGHT_PROG_ROTATE_FWD = 0xd3;
        const byte LIGHT_PROG_ROTATE_BACK = 0xd4;
        const byte LIGHT_PROG_SHOW = 0xd5;
        const byte LIGHT_PROG_RANGE = 0xd6;
        const byte LIGHT_PROG_MODE = 0xd7;
        const byte LIGHT_PROG_MODE1 = 0xd8;

        const byte LIGHT_MODE_REPLACE = 0x00;
        const byte LIGHT_MODE_ADD_RGB = 0x01;
        const byte LIGHT_MODE_SUBTRACT_RGB = 0x02;
        const byte LIGHT_MODE_MULTIPLY_RGB = 0x03;
        const byte LIGHT_MODE_LAST = 0x03;

        const byte LIGHT_PROG_COLN = 0xc0;
        const byte LIGHT_PROG_COL1 = 0xc1;
        const byte LIGHT_PROG_COL2 = 0xc2;
        const byte LIGHT_PROG_COL3 = 0xc3;

        const byte LIGHT_PROG_COL1_SET = 0xcf;
        const ushort LIGHT_PROG_MULT = 0x100;


        static ushort cmdCode(string cmd)
        {
            switch (cmd)
            {
                case "setall":
                    return LIGHT_PROG_SET_ALL;
                case "fade":
                    return LIGHT_PROG_FADE;
                case "fadehsv":
                    return LIGHT_PROG_FADE_HSV;
                case "rotfwd":
                    return LIGHT_PROG_ROTATE_FWD;
                case "rotback":
                    return LIGHT_PROG_ROTATE_BACK;
                case "show":
                case "wait":
                    return LIGHT_PROG_SHOW;
                case "range":
                    return LIGHT_PROG_RANGE;
                case "mode":
                    return LIGHT_PROG_MODE;
                case "tmpmode":
                    return LIGHT_PROG_MODE1;
                case "setone":
                    return LIGHT_PROG_COL1_SET;
                case "mult":
                    return LIGHT_PROG_MULT;
                default:
                    throw new ArgumentException("cmd");
            }
        }

        byte[] outarr;
        int outarrp;
        ArrayList colors;
        int pos = 0;
        ushort currcmd = 0;
        string source;
        ArrayList args;

        private LedPixelEncoder(string source, object[] args)
        {
            this.outarr = new byte[256];
            this.outarrp = 0;
            this.colors = new ArrayList();
            this.pos = 0;
            this.currcmd = 0;
            this.source = source;
            this.args = new System.Collections.ArrayList();
            for (int i = 0; i < args.Length; i++)
                this.args.Add(args[i]);
        }


        /**
         * Encodes a light command into a buffer
         * @param format
         * @param args
         * @returns
         * @category Data Packing
         */
        public static byte[] ToBuffer(string source, object[] args)
        {
            var encoder = new LedPixelEncoder(source, args);
            return encoder.run();
        }

        private void write(byte value)
        {
            this.outarr[this.outarrp++] = value;
        }

        private byte[] run()
        {
            while (this.pos < source.Length)
            {
                var token = this.nextToken();
                var t0 = token[0];
                if (97 <= t0 && t0 <= 122)
                {
                    // a-z
                    this.flush();
                    var currcmd = cmdCode(token);
                    if (currcmd == LIGHT_PROG_MULT)
                    {
                        var f = double.Parse(this.nextToken());
                        if (f < 0 || f > 2)
                            throw new ArgumentException("expecting scale");
                        this.write(0xd8); // tmpmode
                        this.write(3); // mult
                        this.write(0xd0); // setall
                        byte mm = (byte)(((uint)Math.Round(128 * f)) & 0xff);
                        this.write(0xc1);
                        this.write(mm);
                        this.write(mm);
                        this.write(mm);
                    }
                    else
                    {
                        this.write((byte)(this.currcmd & 0xff));
                    }
                }
                else if (48 <= t0 && t0 <= 57)
                {
                    // 0-9
                    this.pushNumber(UInt32.Parse(token));
                }
                else if (t0 == 37)
                {
                    var v = (uint)this.args[0]; // shift
                    this.args.RemoveAt(0);
                    this.pushNumber(v);
                }
                else if (t0 == 35)
                {
                    // #
                    if (token.Length == 1)
                    {
                        var v = this.args[0];
                        this.args.RemoveAt(0);
                        if (Util.IsNumber(v)) this.colors.Add(v);
                        else if (v.GetType().IsArray) foreach (var vv in v as Array) colors.Add(vv);
                        else throw new ArgumentException("v");
                    }
                    else
                    {
                        if (token.Length == 7)
                        {
                            var b = HexEncoding.ToBuffer(token.Substring(1));
                            var c = (b[0] << 16) | (b[1] << 8) | b[2];
                            this.colors.Add(c);
                        }
                        else
                        {
                            throw new ArgumentException("invalid color: " + token);
                        }
                    }
                }
            }
            this.flush();

            var res = new byte[this.outarrp];
            Array.Copy(this.outarr, 0, res, 0, outarrp);
            return res;
        }

        private void pushNumber(uint n)
        {
            if ((n | 0) != n || n < 0 || n >= 16383)
                throw new ArgumentException("number out of range: " + n);
            if (n < 128) this.write((byte)n);
            else
            {
                this.write((byte)(0x80 | (n >> 8)));
                this.write((byte)(n & 0xff));
            }
        }

        private void flush()
        {
            if (currcmd == 0xcf)
            {
                if (this.colors.Count != 1) throw new ArgumentException("setone requires 1 color");
            }
            else
            {
                if (colors.Count == 0) return;
                if (colors.Count <= 3) this.write((byte)(0xc0 | colors.Count));
                else
                {
                    this.write(0xc0);
                    this.write((byte)this.colors.Count);
                }
            }
            foreach (var color in this.colors)
            {
                var c = Util.UnboxUint(color);
                this.write((byte)((c >> 16) & 0xff));
                this.write((byte)((c >> 8) & 0xff));
                this.write((byte)((c >> 0) & 0xff));
            }
            this.colors = new ArrayList();
        }

        private string nextToken()
        {
            while (isWhiteSpace(this.source[pos])) pos++;

            var beg = this.pos;
            while (this.pos < this.source.Length && !isWhiteSpace(source[pos]))
                pos++;
            return source.Substring(beg, pos - beg);
        }

        private static bool isWhiteSpace(char code)
        {
            return code == 32 || code == 13 || code == 10 || code == 9;
        }
    }
}