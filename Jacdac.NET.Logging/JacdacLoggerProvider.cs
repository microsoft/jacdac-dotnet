using Microsoft.Extensions.Logging;
using System;


namespace Jacdac.Logging
{
    /// <summary>
    /// A logger provider that sends logging messages through the Jacdac bus
    /// </summary>
    internal sealed class JacdacLoggerProvider : ILoggerProvider
    {
        readonly JDBus bus;
        JacdacLogger logger;

        public JacdacLoggerProvider(JDBus bus)
        {
            this.bus = bus;
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (this.logger == null)
                this.logger = new JacdacLogger(this.bus);
            return this.logger;
        }

        public void Dispose()
        {
            this.logger = null;
        }
    }
}
