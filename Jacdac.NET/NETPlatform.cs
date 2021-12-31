using DeviceId;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Jacdac.NET
{
    public static class NETPlatform
    {
        public static void Init()
        {
            var deviceId = new DeviceIdBuilder()
                .AddMachineName()
                .AddOsVersion()
                .ToString();
            var sha = System.Security.Cryptography.SHA1.Create();
            sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(deviceId));
            var jid = sha.Hash.Take(8).ToArray();
            Platform.DeviceId = jid;
            Platform.RealTimeClock = RealTimeClockVariant.Computer;
            Platform.CreateClock = () =>
            {
                var start = DateTime.Now;
                return () => DateTime.Now - start;
            };
        }
    }
}