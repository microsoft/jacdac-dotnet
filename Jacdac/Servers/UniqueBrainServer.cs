using System;

namespace Jacdac.Servers
{
    internal sealed class UniqueBrainServer : JDServiceServer
    {
        public UniqueBrainServer()
            : base(ServiceClasses.UniqueBrain, null) { }
    }
}
