using Jacdac.Servers;
using Microsoft.Extensions.Logging;
using System;

namespace Jacdac.Logging
{
    /// <summary>
    /// A ILogger implementation that send log output through Jacdac
    /// </summary>
    internal sealed class JacdacLogger : ILogger
    {
        readonly JDBus bus;

        public JacdacLogger(JDBus bus)
        {
            this.bus = bus;
        }

        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel)
        {
            var server = this.bus.Logger;
            if (server == null) return false;
            var minPriority = server.MinPriority;
            var priority = ToLoggerPriority(logLevel);
            return (byte)logLevel >= (byte)minPriority;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
                return;

            var server = this.bus.Logger;
            if (server == null) return;
            var priority = ToLoggerPriority(logLevel);
            var message = formatter(state, exception);
            server.SendReport(priority, message);
        }

        private static LoggerPriority ToLoggerPriority(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug: return LoggerPriority.Debug; ;
                case LogLevel.Information: return LoggerPriority.Log;
                case LogLevel.Warning: return LoggerPriority.Warning;
                case LogLevel.Error: return LoggerPriority.Error;
                case LogLevel.Critical: return LoggerPriority.Error;
                default: return LoggerPriority.Silent;
            }
        }
    }
}
