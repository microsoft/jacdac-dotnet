using System;
using Microsoft.Extensions.Logging;

namespace Jacdac.Logging
{
    public static class JacdacLoggerExtensions
    {
        /// <summary>
        /// Adds a Jacdac logger provider that sends log messages through the Jacdac bus
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="bus"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddJacdac(this ILoggingBuilder builder, JDBus bus)
        {
            var provider = new JacdacLoggerProvider(bus);
            return builder.AddProvider(provider);
        }
    }
}
