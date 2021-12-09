using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacdac.Services
{
    public static class ServiceRegistry
    {
        private static Dictionary<uint, Type> serviceMappings = new Dictionary<uint, Type>() 
        {
            [0] = typeof(ControlService),
            [0x1473a263] = typeof(ButtonService),
            [0x10fa29c9] = typeof(RotaryEncoderService)
        };

        public static void RegisterService<T>(uint serviceId) where T: JDService {
            serviceMappings.Add(serviceId, typeof(T));
        }

        public static bool HasService(uint serviceId)
        {
            return serviceMappings.ContainsKey(serviceId);
        }

        public static Type GetService(uint serviceId)
        {
            return serviceMappings[serviceId];
        }

        public static JDService InstantiateService(uint serviceId, byte serviceIndex, JDDevice device)
        {
            if(HasService(serviceId))
                return (JDService)Activator.CreateInstance(GetService(serviceId), serviceId, serviceIndex, device);
            return new JDService(serviceId, serviceIndex, device);
        }
    }
}
