using System;

namespace Jacdac.Servers
{
    internal sealed class InfrastructureServer : JDServiceServer
    {
        public InfrastructureServer()
            : base(ServiceClasses.Infrastructure, null) { }
    }
}
