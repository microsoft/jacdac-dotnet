namespace Jacdac
{
    public delegate ushort Crc16Calculator(byte[] p, int start, int size);

    public static class Platform
    {
        public static Crc16Calculator Crc16 = (byte[] p, int start, int size) =>
        {
            ushort crc = 0xffff;
            for (var i = start; i < start + size; ++i)
            {
                var data = p[i];
                var x = (crc >> 8) ^ data;
                x ^= x >> 4;
                crc = (ushort)((crc << 8) ^ (x << 12) ^ (x << 5) ^ x);
                crc &= 0xffff;
            }
            return crc;
        };
    }
}
