using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jacdac.Servers
{
    internal sealed class UniqueBrainServer : JDServiceServer
    {
        public UniqueBrainServer()
            : base(ServiceClasses.UniqueBrain, null) { }
    }
}
