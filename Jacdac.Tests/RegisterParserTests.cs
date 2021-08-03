using System;
using Xunit;

namespace Jacdac.Tests
{
    public class RegisterParserTests
    {
        [Fact]
        public void ParseSimpleBuffer()
        {
            var buffer = new byte[] { 0x1, 0x2, 0x3, 0x4 };
            var format = "b";
            var parsed = RegisterParser.ParseBuffer(format, buffer);

            Assert.True(parsed.IsSingleValue);
            Assert.IsType<byte[]>(parsed.Value);
            Assert.Equal<byte[]>(parsed.Value, buffer);
        }

        [Fact]
        public void ParseSimpleNumber()
        {
            var buffer = new byte[] { 0x92, 0x10, 0x0, 0x0 };
            var format = "u32";
            var parsed = RegisterParser.ParseBuffer(format, buffer);

            Assert.True(parsed.IsSingleValue);
            Assert.IsType<UInt32>(parsed.Value);
            Assert.Equal<UInt32>(4242, parsed.GetValue<UInt32>());
        }

        [Fact]
        public void ParseTwoSimpleNumbers()
        {
            var buffer = new byte[] { 0x92, 0x10, 0x0, 0x0, 0xCF, 0x7, 0x0, 0x0 };
            var format = "u32 u32";
            var parsed = RegisterParser.ParseBuffer(format, buffer);

            Assert.True(2 == parsed.Values.Length);
            Assert.True(4242 == parsed.GetValue<uint>(0));
            Assert.True(1999 == parsed.GetValue<uint>(1));
        }

        [Fact]
        public void ParseFloat()
        {
            var buffer = new byte[] { 0x7F, 0xFF };
            var format = "u8.8";
            var parsed = RegisterParser.ParseBuffer(format, buffer);

            Assert.True(parsed.IsSingleValue);
            Assert.IsType<float>(parsed.Value);
            Assert.Equal<float>(255.4961f, parsed.Value);
        }

        [Fact]
        public void ParseString()
        {
            var buffer = new byte[] { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100 };
            var format = "s";
            var parsed = RegisterParser.ParseBuffer(format, buffer);

            Assert.True(parsed.IsSingleValue);
            Assert.IsType<string>(parsed.Value);
            Assert.Equal("Hello World", parsed.Value);
        }

        [Fact]
        public void ParseExhaustingArray()
        {
            var buffer = new byte[] { 0x7F, 0xCD, 0xAB, 0x34, 0x12, 0x78, 0x56 };
            var format = "u8 u16[]";
            var parsed = RegisterParser.ParseBuffer(format, buffer);

            // 43981 4660 22136

            Assert.False(parsed.IsSingleValue);
            Assert.IsType<byte>(parsed.Values[0]);
            Assert.Equal<byte>(127, parsed.GetValue<byte>(0));

            Assert.IsType<ushort>(parsed.Values[1]);
            Assert.Equal<ushort>(43981, parsed.GetValue<ushort>(1));
            Assert.Equal<ushort>(4660, parsed.GetValue<ushort>(2));
            Assert.Equal<ushort>(22136, parsed.GetValue<ushort>(3));
        }

        [Fact]
        public void ParseSizeArray()
        {
            var buffer = new byte[] { 0x7F, 0xCD, 0xAB, 0x34, 0x12, 0xFF };
            var format = "u8 u16[2] u8";
            var parsed = RegisterParser.ParseBuffer(format, buffer);

            // 43981 4660 22136

            Assert.False(parsed.IsSingleValue);
            Assert.IsType<byte>(parsed.Values[0]);
            Assert.Equal<byte>(127, parsed.GetValue<byte>(0));

            Assert.IsType<object[]>(parsed.Values[1]);
            Assert.IsType<ushort>(parsed.Values[1][0]);
            Assert.Equal<ushort>(43981, (ushort) parsed.GetValue<object[]>(1)[0]);
            Assert.Equal<ushort>(4660, (ushort) parsed.GetValue<object[]>(1)[1]);

            Assert.IsType<byte>(parsed.Values[2]);
            Assert.Equal<byte>(255, parsed.GetValue<byte>(2));
        }

        [Theory]
        [InlineData(new byte[] {0xFF, 0xFF}, "u8 u8", new object[] { 255, 255 })]
        [InlineData(
            new byte[] { 0x7F, 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100 }, 
            "u8 s", 
            new object[] { 127, "Hello World" })]
        [InlineData(
            new byte[] { 0x7F, 0xFF, 0x15, 0x1, 0x3B, 0x1, 0xEF, 0xEF },
            "u8 u8 r: u16",
            new object[] { 127, 255, 277, 315, 61423 })]
        public void ParseArbitrary(byte[] buffer, string format, dynamic[] expectedValues)
        {
            var parsed = RegisterParser.ParseBuffer(format, buffer);
            for(int i = 0; i < expectedValues.Length; i++)
            {
                Assert.True(parsed.Values[i] == expectedValues[i]);
            }
        }
    }
}
