using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacdac.Services
{
    [AttributeUsage(AttributeTargets.Method)]
    public class JDReadRegister : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Event)]
    public class JDEvent : Attribute
    {
    }
}
