using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacdac.Services
{
    public class ControlService : JDService
    {
        public ControlService(uint serviceId, byte serviceIndex, JDDevice device) : base(serviceId, serviceIndex, device)
        {
        }
        public async Task Identify()
        {
            await Device.SendPacket(BuildAction(0x81));
        }

        [JDReadRegister]
        public async Task<string> GetDeviceDescription() => await ReadStringRegister(0x180);

        [JDReadRegister]
        public async Task<string> GetDeviceUrl() => await ReadStringRegister(0x181);

        [JDReadRegister]
        public async Task<int> GetFirmwareIdentifier() => await ReadInt32Register(0x181);

        [JDReadRegister]
        public async Task<string> GetFirmwareVersion() => await ReadStringRegister(0x185);

        [JDReadRegister]
        public async Task<long> GetUptime() => await ReadInt64Register(0x186);

        [JDReadRegister]
        public async Task<long> GetMCUTemperature() => await ReadInt16Register(0x182);

        public struct AnnouncementReport
        {
            public AnnounceFlags AnnounceFlags { get; private set; }

            public byte PacketCount { get; private set; }

            public uint[] Services { get; private set; }

            public static AnnouncementReport Parse(byte[] data)
            {
                var serviceCount = (data.Length - 4) / 4;
                var services = new uint[serviceCount];
                for (int i = 1; i <= serviceCount; i++)
                    services[i - 1] = (uint)(data[i * 4] | (data[i * 4 + 1] << 8) | (data[i * 4 + 2] << 16) | (data[i * 4 + 3] << 24));

                return new AnnouncementReport
                {
                    AnnounceFlags = (AnnounceFlags)(data[0] | (data[1] << 8)),
                    PacketCount = data[2],
                    Services = services
                };
            }

            public override string ToString()
            {
                return $"[Announcement] PacketCount: {PacketCount}";
            }
        }

        [Flags]
        public enum AnnounceFlags
        {
            RestartCounterSteady = 0x000F,
            RestartCounter1 = 0x0001,
            RestartCounter2 = 0x0002,
            RestartCounter4 = 0x0004,
            RestartCounter8 = 0x0008,
            StatusLightNone = 0x0000,
            StatusLightMono = 0x0010,
            StatusLightRgbNoFade = 0x0020,
            StatusLightRgbFade = 0x0030,
            SupportsACK = 0x0100,
            SupportsBroadcast = 0x0200,
            SupportsFrames = 0x0400,
            IsClient = 0x0800,
        }
    }
}
