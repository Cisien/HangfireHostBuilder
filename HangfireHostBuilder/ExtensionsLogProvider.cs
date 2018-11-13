using Hangfire.Logging;
using Microsoft.Extensions.Logging;
using System;

namespace ConsoleApp5
{
    public class ExtensionsLogProvider : ILogProvider
    {
        private readonly ILoggerFactory _loggerFactory;

        public ExtensionsLogProvider(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            _loggerFactory = loggerFactory;
        }

        public ILog GetLogger(string name)
        {
            return new ExtensionsLog(_loggerFactory.CreateLogger(name));
        }
    }
}
