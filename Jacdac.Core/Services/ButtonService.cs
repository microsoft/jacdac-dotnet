using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacdac.Services
{
    public class ButtonService : SensorService
    {
        public ButtonService(uint serviceId, byte serviceIndex, JDDevice device) : base(serviceId, serviceIndex, device)
        {
        }

        [JDReadRegister]
        public async Task<short> GetPressure() => await ReadInt16Register(0x101);

        [JDReadRegister]
        public async Task<bool> IsPressed() => await ReadBoolRegister(0x181);

        [JDReadRegister]
        public async Task<bool> IsAnalog() => await ReadBoolRegister(0x180);

        public override void HandleEvent(JDPacket packet)
        {
            switch (packet.EventCode)
            {
                case CommonEvents.Active:
                    OnButtonDown?.Invoke();
                    break;

            }
        }

        [JDEvent]
        public event Action OnButtonDown;
    }
}
