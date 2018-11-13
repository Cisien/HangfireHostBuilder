using Hangfire.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using System;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ConsoleApp5
{
    internal class ExtensionsLog : ILog
    {
        private static readonly Func<object, Exception, string> MessageFormatterFunc = MessageFormatter;
        private static readonly object[] EmptyArgs = new object[0];

        private readonly ILogger _targetLogger;

        public ExtensionsLog(ILogger targetLogger)
        {
            if (targetLogger == null) throw new ArgumentNullException(nameof(targetLogger));
            _targetLogger = targetLogger;
        }

        public bool Log(Hangfire.Logging.LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
        {
            var targetLogLevel = ToTargetLogLevel(logLevel);

            // When messageFunc is null, Hangfire.Logging
            // just determines is logging enabled.
            if (messageFunc == null)
            {
                return _targetLogger.IsEnabled(targetLogLevel);
            }

            _targetLogger.Log(targetLogLevel, 0, CreateStateObject(messageFunc()), exception, MessageFormatterFunc);
            return true;
        }

        private static LogLevel ToTargetLogLevel(Hangfire.Logging.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case Hangfire.Logging.LogLevel.Trace:
                    return LogLevel.Trace;
                case Hangfire.Logging.LogLevel.Debug:
                    return LogLevel.Debug;
                case Hangfire.Logging.LogLevel.Info:
                    return LogLevel.Information;
                case Hangfire.Logging.LogLevel.Warn:
                    return LogLevel.Warning;
                case Hangfire.Logging.LogLevel.Error:
                    return LogLevel.Error;
                case Hangfire.Logging.LogLevel.Fatal:
                    return LogLevel.Critical;
            }

            return LogLevel.None;
        }

        private static object CreateStateObject(string message)
        {
            return new FormattedLogValues(message, EmptyArgs);
        }

        private static string MessageFormatter(object state, Exception exception)
        {
            return state.ToString();
        }
    }
}
