using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacdac.Services
{
    public abstract class SensorService : JDService
    {
        public SensorService(uint serviceId, byte serviceIndex, JDDevice device) : base(serviceId, serviceIndex, device)
        {
        }
    }
}
