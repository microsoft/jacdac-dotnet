using System;
using Xunit;

namespace Jacdac.Tests
{
    public class PackTest
    {
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
