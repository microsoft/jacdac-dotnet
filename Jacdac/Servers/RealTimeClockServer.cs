using System;

namespace Jacdac.Servers
{
    public delegate DateTime RealTimeClockLocalTime();

    public sealed class RealTimeClockServer : JDServiceServer
    {
        private readonly RealTimeClockLocalTime localTime;
        public RealTimeClockServer(RealTimeClockLocalTime localTime, RealTimeClockVariant variant)
            : base(Jacdac.RealTimeClockConstants.ServiceClass)
        {
            this.localTime = localTime;
            this.AddRegister(new JDDynamicRegisterServer((ushort)Jacdac.RealTimeClockReg.LocalTime, "u16 u8 u8 u8 u8 u8 u8", (server) =>
            {
                var now = this.localTime();
                return new object[]
                {
                    (ushort)now.Year,
                    (byte)now.Month,
                    (byte)now.Day,
                    (byte)now.DayOfWeek,
                    (byte)now.Hour,
                    (byte)now.Minute,
                    (byte)now.Second
                };
            }));
            if (variant > 0)
                this.AddRegister(new JDStaticRegisterServer((ushort)Jacdac.RealTimeClockReg.Variant, "u8", new object[] { variant }));
        }
    }
}
