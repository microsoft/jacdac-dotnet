using System;

namespace Jacdac.Servers
{
    internal sealed class ProxyServer : JDServiceServer
    {
        public ProxyServer()
            : base(ServiceClasses.Proxy, null) { }
    }
}
