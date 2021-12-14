using System;
using Xunit;

namespace Jacdac.Tests
{
    public class PackTest
    {
        [Fact]
        public void ParseSimpleBuffer()
        {
            var buffer = new byte[] { 0x1, 0x2, 0x3, 0x4 };
            var format = "b";
            var parsed = PacketEncoding.UnPack(format, buffer);

            Assert.True(parsed.Length == 1);
            Assert.IsType<byte[]>(parsed[0]);
            Assert.Equal<byte[]>(parsed[0] as byte[], buffer);
        }

        [Fact]
        public void ParseU16()
        {
            var buffer = new byte[] { 0x92, 0x93 };
            var format = "u16";
            var parsed = PacketEncoding.UnPack(format, buffer);

            Assert.True(parsed.Length == 1);
            Assert.IsType<ushort>(parsed[0]);
            Assert.Equal<ushort>(37778, (ushort)parsed[0]);
        }

        [Fact]
        public void ParseU8()
        {
            var buffer = new byte[] { 0x92 };
            var format = "u8";
            var parsed = PacketEncoding.UnPack(format, buffer);

            Assert.True(parsed.Length == 1);
            Assert.IsType<byte>(parsed[0]);
            Assert.Equal<byte>(0x92, (byte)parsed[0]);
        }

        [Fact]
        public void ParseU32()
        {
            var buffer = new byte[] { 0x92, 0x10, 0x0, 0x0 };
            var format = "u32";
            var parsed = PacketEncoding.UnPack(format, buffer);

            Assert.True(parsed.Length == 1);
            Assert.IsType<UInt32>(parsed[0]);
            Assert.Equal<UInt32>(4242, (uint)parsed[0]);
        }

        [Fact]
        public void ParseTwoSimpleNumbers()
        {
            var buffer = new byte[] { 0x92, 0x10, 0x0, 0x0, 0xCF, 0x7, 0x0, 0x0 };
            var format = "u32 u32";
            var parsed = PacketEncoding.UnPack(format, buffer);

            Assert.True(2 == parsed.Length);
            Assert.True(4242 == (uint)parsed[0]);
            Assert.True(1999 == (uint)parsed[1]);
        }

        [Fact]
        public void ParseFloat()
        {
            var buffer = new byte[] { 0x7F, 0xFF };
            var format = "u8.8";
            var parsed = PacketEncoding.UnPack(format, buffer);

            Assert.True(parsed.Length == 1);
            Assert.IsType<float>(parsed[0]);
            Assert.Equal<float>(255.4961f, (float)parsed[0]);
        }

        [Fact]
        public void ParseString()
        {
            var buffer = new byte[] { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100 };
            var format = "s";
            var parsed = PacketEncoding.UnPack(format, buffer);

            Assert.True(parsed.Length == 1);
            Assert.IsType<string>(parsed[0]);
            Assert.Equal("Hello World", parsed[0]);
        }

        [Fact]
        public void ParseTerminatedString()
        {
            var buffer = new byte[] { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 0, 0xFF };
            var format = "z u8";
            var parsed = PacketEncoding.UnPack(format, buffer);

            Assert.True(parsed.Length == 2);
            Assert.IsType<string>(parsed[0]);
            Assert.IsType<byte>(parsed[1]);
            Assert.Equal("Hello World", parsed[0]);
            Assert.Equal<byte>(255, (byte)parsed[1]);
        }

        [Fact]
        public void ParseExhaustingArray()
        {
            var buffer = new byte[] { 0x7F, 0xCD, 0xAB, 0x34, 0x12, 0x78, 0x56 };
            var format = "u8 u16[]";
            var parsed = PacketEncoding.UnPack(format, buffer);

            Assert.True(parsed.Length == 2);
            Assert.IsType<byte>(parsed[0]);
            Assert.Equal<byte>(127, (byte)parsed[0]);

            Assert.IsType<object[]>(parsed[1]);
            Assert.Equal<ushort>(43981, (ushort)((object[])parsed[1])[0]);
            Assert.Equal<ushort>(4660, (ushort)((object[])parsed[1])[1]);
            Assert.Equal<ushort>(22136, (ushort)((object[])parsed[1])[2]);
        }

        [Fact]
        public void ParseSizeArray()
        {
            var buffer = new byte[] { 0x7F, 0xCD, 0xAB, 0x34, 0x12, 0xFF };
            var format = "u8 x[4] u8";
            var parsed = PacketEncoding.UnPack(format, buffer);

            Assert.True(parsed.Length == 2);
            Assert.IsType<byte>(parsed[0]);
            Assert.Equal<byte>(127, (byte)parsed[0]);

            Assert.IsType<byte>(parsed[1]);
            Assert.Equal<byte>(255, (byte)parsed[1]);
        }

        const int precision = 4;

        [Theory]
        [InlineData("u8", new object[] { (byte)42 })]
        [InlineData("u16", new object[] { (ushort)42 })]
        [InlineData("u32", new object[] { (uint)42 })]
        [InlineData("i8", new object[] { (sbyte)-3 })]
        [InlineData("i16", new object[] { (short)-42 })]
        [InlineData("i32", new object[] { (int)-42 })]
        [InlineData("u16 u16 i16", new object[] { (ushort)42, (ushort)77, (short)-10 })]
        [InlineData("u16 z s", new object[] { (ushort)42, "foo", "bar" })]
        [InlineData("u32 z s", new object[] { (uint)42, "foo", "bar" })]
        [InlineData("i8 z s", new object[] { (sbyte)42, "foo", "bar" })]
        [InlineData("u8 z s", new object[] { (byte)42, "foo12", "bar" })]
        [InlineData("u8 r: u8 z", new object[] {
            (byte)42,
            new object[] {
                new object[] { (byte)17, "xy" },
                new object[] { (byte)18, "xx" }}
            })]
        [InlineData("z b", new object[] { "foo12", new byte[] { 0, 1, 2, 3, 4 } })]
        [InlineData("u16 r: u16", new object[] { (ushort)42, new object[] { new object[] { (ushort)17 }, new object[] { (ushort)18 } } })]
        [InlineData("i8 s[9] u16 s[10] u8", new object[] { (sbyte)-100, "foo", (ushort)1000, "barbaz", (byte)250 })]
        [InlineData("i8 x[4] s[9] u16 x[2] s[10] x[3] u8", new object[] { (sbyte)-100, "foo", (ushort)1000, "barbaz", (byte)250 })]
        [InlineData("u16 u16[]", new object[] { (ushort)42, new object[] { (ushort)17, (ushort)18 } })]
        [InlineData("u16 u16[]", new object[] { (ushort)42, new object[] { (ushort)18 } })]
        [InlineData("u16 u16[]", new object[] { (ushort)42, new object[] { } })]
        [InlineData("u0.16", new object[] { (float)0 }, precision)]
        [InlineData("u0.16", new object[] { (float)0.42 }, precision)]
        [InlineData("u0.16", new object[] { (float)1 }, precision)]
        [InlineData("i1.15", new object[] { (float)0 }, precision)]
        [InlineData("i1.15", new object[] { (float)1 }, precision)]
        [InlineData("i1.15", new object[] { (float)-1 }, precision)]
        [InlineData("i16.16", new object[] { (float)0.1}, precision)]
        [InlineData("i16.16", new object[] { (float)1 }, precision)]
        [InlineData("i16.16", new object[] { (float)Math.PI }, precision) ]
        public void TestOne(string fmt, object[] values, int precision = 0)
        {
            var packed = PacketEncoding.Pack(fmt, values);
            Console.WriteLine(packed);

            var unpacked = PacketEncoding.UnPack(fmt, packed);
            Console.WriteLine(unpacked);

            var repacked = PacketEncoding.Pack(fmt, unpacked);
            Console.WriteLine(repacked);

            Assert.True(packed != null);
            Assert.Equal(values.Length, unpacked.Length);
            for (int i = 0; i < values.Length; i++)
            {
                if (precision > 0)
                    Assert.Equal((float)values[i], (float)unpacked[i], precision);
                else
                    Assert.Equal(values[i], unpacked[i]);
            }
            Assert.Equal(HexEncoding.ToString(packed), HexEncoding.ToString(repacked));
        }
    }
}
