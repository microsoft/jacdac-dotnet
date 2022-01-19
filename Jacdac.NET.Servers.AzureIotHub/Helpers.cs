// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Security.Authentication;

namespace Jacdac.Servers
{
    // Sample exception handler class - this class should be modified based on your application's logic
    internal class ExceptionHelper
    {
        private static readonly HashSet<Type> s_networkExceptions = new HashSet<Type>
        {
            typeof(IOException),
            typeof(SocketException),
            typeof(ClosedChannelException),
            typeof(TimeoutException),
            typeof(OperationCanceledException),
            typeof(HttpRequestException),
            typeof(WebException),
            typeof(WebSocketException),
        };

        private static bool IsNetwork(Exception singleException)
        {
            return s_networkExceptions.Any(baseExceptionType => baseExceptionType.IsInstanceOfType(singleException));
        }

        internal static bool IsNetworkExceptionChain(Exception exceptionChain)
        {
            return exceptionChain.Unwind(true).Any(e => IsNetwork(e) && !IsTlsSecurity(e));
        }

        internal static bool IsSecurityExceptionChain(Exception exceptionChain)
        {
            return exceptionChain.Unwind(true).Any(e => IsTlsSecurity(e));
        }

        private static bool IsTlsSecurity(Exception singleException)
        {
            if (// WinHttpException (0x80072F8F): A security error occurred.
                (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && (singleException.HResult == unchecked((int)0x80072F8F))) ||
                // CURLE_SSL_CACERT (60): Peer certificate cannot be authenticated with known CA certificates.
                (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && (singleException.HResult == 60)) ||
                singleException is AuthenticationException)
            {
                return true;
            }

            return false;
        }
    }
    internal static class ExceptionExtensions
    {
        internal static IEnumerable<Exception> Unwind(this Exception exception, bool unwindAggregate = false)
        {
            while (exception != null)
            {
                yield return exception;

                if (!unwindAggregate)
                {
                    exception = exception.InnerException;
                    continue;
                }

                if (exception is AggregateException aggEx
                    && aggEx.InnerExceptions != null)
                {
                    foreach (Exception ex in aggEx.InnerExceptions)
                    {
                        foreach (Exception innerEx in ex.Unwind(true))
                        {
                            yield return innerEx;
                        }
                    }
                }

                exception = exception.InnerException;
            }
        }
    }

    /// <summary>
    /// An exponential backoff based retry policy that retries on encountering transient exceptions.
    /// </summary>
    internal class ExponentialBackoffTransientExceptionRetryPolicy : IRetryPolicy
    {
        private static readonly Random s_random = new Random();
        private static readonly TimeSpan s_minBackoff = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan s_maxBackoff = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan s_deltaBackoff = TimeSpan.FromMilliseconds(100);

        private static int s_maxRetryCount;
        private static IDictionary<Type, string> s_exceptionsToBeIgnored;

        internal ExponentialBackoffTransientExceptionRetryPolicy(int maxRetryCount = default, IDictionary<Type, string> exceptionsToBeIgnored = default)
        {
            s_maxRetryCount = maxRetryCount == 0 ? int.MaxValue : maxRetryCount;
            s_exceptionsToBeIgnored = exceptionsToBeIgnored;
        }

        public bool ShouldRetry(int currentRetryCount, Exception lastException, out TimeSpan retryInterval)
        {
            if (currentRetryCount < s_maxRetryCount)
            {
                if ((lastException is IotHubException iotHubException && iotHubException.IsTransient)
                    || ExceptionHelper.IsNetworkExceptionChain(lastException)
                    || (s_exceptionsToBeIgnored != null && s_exceptionsToBeIgnored.ContainsKey(lastException.GetType())))
                {
                    double exponentialInterval =
                        (Math.Pow(2.0, currentRetryCount) - 1.0)
                        * s_random.Next(
                            (int)s_deltaBackoff.TotalMilliseconds * 8 / 10,
                            (int)s_deltaBackoff.TotalMilliseconds * 12 / 10)
                        + s_minBackoff.TotalMilliseconds;

                    double maxInterval = s_maxBackoff.TotalMilliseconds;
                    double num2 = Math.Min(exponentialInterval, maxInterval);
                    retryInterval = TimeSpan.FromMilliseconds(num2);
                    return true;
                }
            }

            retryInterval = TimeSpan.Zero;
            return false;
        }
    }

    /// <summary>
    /// A helper class with methods that aid in retrying operations.
    /// </summary>
    internal class RetryOperationHelper
    {
        /// <summary>
        /// Retry an async operation on encountering a transient operation. The retry strategy followed is an exponential backoff strategy.
        /// </summary>
        /// <param name="operationName">An identifier for the async operation to be executed. This is used for debugging purposes.</param>
        /// <param name="asyncOperation">The async operation to be retried.</param>
        /// <param name="shouldExecuteOperation">A function that determines if the operation should be executed.
        /// Eg.: for scenarios when we want to execute the operation only if the client is connected, this would be a function that returns if the client is currently connected.</param>
        /// <param name="logger">The <see cref="ILogger"/> instance to be used.</param>
        /// <param name="exceptionsToBeIgnored">An optional list of exceptions that can be ignored.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        internal static async Task RetryTransientExceptionsAsync(
            string operationName,
            Func<Task> asyncOperation,
            Func<bool> shouldExecuteOperation,
            ILogger logger,
            IDictionary<Type, string> exceptionsToBeIgnored = default,
            CancellationToken cancellationToken = default)
        {
            IRetryPolicy retryPolicy = new ExponentialBackoffTransientExceptionRetryPolicy(maxRetryCount: int.MaxValue, exceptionsToBeIgnored: exceptionsToBeIgnored);

            int attempt = 0;
            bool shouldRetry;
            do
            {
                Exception lastException = new IotHubCommunicationException("Client is currently reconnecting internally; attempt the operation after some time.");
                try
                {
                    if (shouldExecuteOperation())
                    {
                        logger?.LogInformation(FormatRetryOperationLogMessage(operationName, attempt, "executing."));

                        await asyncOperation();
                        break;
                    }
                    else
                    {
                        logger?.LogWarning(FormatRetryOperationLogMessage(operationName, attempt, "operation is not ready to be executed. Attempt discarded."));
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogWarning(FormatRetryOperationLogMessage(operationName, attempt, $"encountered an exception while processing the request: {ex}"));
                    lastException = ex;
                }

                shouldRetry = retryPolicy.ShouldRetry(++attempt, lastException, out TimeSpan retryInterval);
                if (shouldRetry)
                {
                    logger.LogWarning(FormatRetryOperationLogMessage(operationName, attempt, $"caught a recoverable exception, will retry in {retryInterval}."));
                    await Task.Delay(retryInterval);

                }
                else
                {
                    logger?.LogWarning(FormatRetryOperationLogMessage(operationName, attempt, $"retry policy determined that the operation should no longer be retried, stopping retries."));
                }
            }
            while (shouldRetry && !cancellationToken.IsCancellationRequested);
        }

        private static string FormatRetryOperationLogMessage(string operationName, int attempt, string logMessage)
        {
            return $"Operation name = {operationName}, attempt = {attempt}, status = {logMessage}";
        }
    }
}