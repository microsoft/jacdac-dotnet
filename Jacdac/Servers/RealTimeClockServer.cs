using System;

namespace Jacdac.Servers
{
    public sealed class RealTimeClockServer : JDServer
    {
        public RealTimeClockServer(RealTimeClockVariant variant)
            : base(Jacdac.RealTimeClockConstants.ServiceClass)
        {
            this.AddRegister(new JDDynamicRegisterServer((ushort)Jacdac.RealTimeClockReg.LocalTime, "u16 u8 u8 u8 u8 u8 u8", RealTimeClockServer.localTime));
            if (variant > 0)
                this.AddRegister(new JDStaticRegisterServer((ushort)Jacdac.RealTimeClockReg.Variant, "u8", new object[] { variant }));
        }

        static object[] localTime(JDRegisterServer server)
        {
            var now = DateTime.Now;
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
        }
    }
}
