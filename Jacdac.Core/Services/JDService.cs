using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacdac.Services
{
    public class JDService
    {
        public uint ServiceId { get; }
        public byte ServiceIndex { get; }
        public JDDevice Device { get; }

        public PacketFlags DefaultFlags => (Device.DeviceFlags.HasFlag(ControlService.AnnounceFlags.SupportsACK) ? PacketFlags.AckRequested : 0) | PacketFlags.CommandPacket;

        public JDService(uint serviceId, byte serviceIndex, JDDevice device)
        {
            ServiceId = serviceId;
            ServiceIndex = serviceIndex;
            Device = device;
        }

        public async Task<byte[]> ReadRegister(ushort registerId)
        {
            try
            {
                var result = await Device.SendPacketWithResponse(BuildRegisterRead(registerId));
                return result.Data;
            } catch (Exception ex)
            {
                throw new Exception("Could not read register.", ex);
            }
        }

        public async Task<string> ReadStringRegister(ushort registerId) => Encoding.UTF8.GetString(await ReadRegister(registerId));

        public async Task<bool> ReadBoolRegister(ushort registerId)
        {
            var result = await ReadRegister(registerId);
            if (result.Length == 0)
                return false;
            return result[0] == 1;
        }

        public async Task<short> ReadInt16Register(ushort registerId)
        {
            var result = await ReadRegister(registerId);
            if (result.Length < 2)
                return 0;
            return BitConverter.ToInt16(result);
        }


        public async Task<int> ReadInt32Register(ushort registerId) {
            var result = await ReadRegister(registerId);
            if (result.Length < 4)
                return 0;
            return BitConverter.ToInt32(result);
        }

        public async Task<long> ReadInt64Register(ushort registerId)
        {
            var result = await ReadRegister(registerId);
            if (result.Length < 8)
                return 0;
            return BitConverter.ToInt64(result);
        }

        [JDReadRegister]
        public async Task<string> GetInstanceName() => await ReadStringRegister(0x109);

        public JDPacket BuildAction(ushort serviceCommand, byte[]? data = null)
        {
            return new JDPacket(Device.DeviceIdentifier, ServiceIndex, serviceCommand, data, DefaultFlags);
        }

        public JDPacket BuildRegisterRead(ushort registerId)
        {
            return new JDPacket(Device.DeviceIdentifier, ServiceIndex, (ushort)(0x1000 | registerId), null, DefaultFlags);
        }

        public JDPacket BuildRegisterWrite(ushort registerId, byte[] data)
        {
            return new JDPacket(Device.DeviceIdentifier, ServiceIndex, (ushort)(0x2000 | registerId), data, DefaultFlags);
        }

        public virtual void HandleEvent(JDPacket packet) { }

        public static class CommonEvents
        {
            public const byte Active = 0x01;
            public const byte Inactive = 0x02;
            public const byte Change = 0x03;
            public const byte Neutral = 0x07;
        }
    }
}
