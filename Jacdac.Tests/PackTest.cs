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

        const double err = 1e-4;

        [Theory]
        [InlineData("u16", new object[] { 42 })]
        [InlineData("u8", new object[] { 42 })]
        [InlineData("u32", new object[] { 42 })]
        [InlineData("u16 u16 i16", new object[] { 42, 77, -10 })]
        [InlineData("u16 z s", new object[] { 42, "foo", "bar" })]
        [InlineData("u32 z s", new object[] { 42, "foo", "bar" })]
        [InlineData("i8 z s", new object[] { 42, "foo", "bar" })]
        [InlineData("u8 z s", new object[] { 42, "foo12", "bar" })]
        [InlineData("u8 r: u8 z", new object[] {42,    new object[] {
        new object[] { 17, "xy" },        new object[] { 18, "xx" }}
        })]
        [InlineData("z b", new object[] { "foo12", new byte[] { 0, 1, 2, 3, 4 } })]
        [InlineData("u16 r: u16", new object[] { 42, new object[] { new object[] { 17 }, new object[] { 18 } } })]
        [InlineData("i8 s[9] u16 s[10] u8", new object[] { -100, "foo", 1000, "barbaz", 250 })]
        [InlineData("i8 x[4] s[9] u16 x[2] s[10] x[3] u8", new object[] { -100, "foo", 1000, "barbaz", 250 })]
        [InlineData("u16 u16[]", new object[] { 42, new object[] { 17, 18 } })]
        [InlineData("u16 u16[]", new object[] { 42, new object[] { 18 } })]
        [InlineData("u16 u16[]", new object[] { 42, new object[] { } })]
        [InlineData("u16 z[]", new object[] { 42, new object[] { "foo", "bar", "bz" } })]
        /*
        [InlineData("u0.16", [0], { maxError: err })]
        [InlineData("u0.16", [0.42], { maxError: err })]
        [InlineData("u0.16", [1], { maxError: err })]
        [InlineData("i1.15", [0], { maxError: err })]
        [InlineData("i1.15", [1], { maxError: err })]
        [InlineData("i1.15", [-1], { maxError: err })]
     [InlineData(
        "b[8] u32 u8 s",
        [fromHex(`a1b2c3d4e5f6a7b8`), 0x12345678, 0x42, "barbaz"],
        { expectedPayload: "a1b2c3d4e5f6a7b8785634124262617262617a" }
    )]
    [InlineData("i16.16", [0.1], { maxError: err })]
    [InlineData("i16.16", [1], { maxError: err })]
    [InlineData("i16.16", [Math.PI], { maxError: err }) ]*/
        public void TestOne(string fmt, object[] values)
        {
        }
    }
}
