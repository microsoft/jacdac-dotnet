using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacdac.Services
{
    public class RotaryEncoderService : SensorService
    {
        public RotaryEncoderService(uint serviceId, byte serviceIndex, JDDevice device) : base(serviceId, serviceIndex, device)
        {
        }

        [JDReadRegister]
        public async Task<int> GetPosition() => await ReadInt32Register(0x101);

        [JDReadRegister]
        public async Task<short> GetClicksPerTurn() => await ReadInt16Register(0x180);
    }
}
