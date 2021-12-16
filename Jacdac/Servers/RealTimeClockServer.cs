using System;

namespace Jacdac.Servers
{
    public delegate DateTime RealTimeClockLocalTime();
    public delegate void SetTimeHandler(DateTime now);

    public class RealTimeClockServerOptions : JDServiceServerOptions
    {
        public RealTimeClockVariant Variant;
        public SetTimeHandler SetTime;
    }

    public sealed class RealTimeClockServer : JDServiceServer
    {
        private readonly RealTimeClockLocalTime localTime;
        private readonly SetTimeHandler setTime;

        public RealTimeClockServer(RealTimeClockLocalTime localTime, RealTimeClockServerOptions options)
            : base(Jacdac.RealTimeClockConstants.ServiceClass, options)
        {
            this.localTime = localTime;
            this.setTime = options.SetTime;
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
            this.AddRegister(new JDStaticRegisterServer((ushort)Jacdac.SensorReg.StreamingInterval, "u32", new object[] { 60000u }));
            if (options != null && options.Variant > 0)
                this.AddRegister(new JDStaticRegisterServer((ushort)Jacdac.RealTimeClockReg.Variant, "u8", new object[] { options.Variant }));
            if (options != null && options.SetTime != null)
                this.AddCommand((ushort)Jacdac.RealTimeClockCmd.SetTime, this.handleSetTime);
        }

        private void handleSetTime(JDNode sender, PacketEventArgs args)
        {
            var values = PacketEncoding.UnPack("u16 u8 u8 u8 u8 u8 u8", args.Packet.Data);
            var year = (ushort)values[0];
            var month = (byte)values[1];
            var dayOfMonth = (byte)values[2];
            var dayOfWeek = (byte)values[3];
            var hour = (byte)values[4];
            var minute = (byte)values[5];
            var second = (byte)values[6];

            var now = new DateTime(year, month, dayOfMonth, hour, minute, second);
            this.setTime(now);
            // update bus time
        }
    }
}
