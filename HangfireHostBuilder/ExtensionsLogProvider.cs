using Hangfire.Logging;
using Microsoft.Extensions.Logging;
using System;

namespace HangfireHostBuilder
{
    public class ExtensionsLogProvider : ILogProvider
    {
        private readonly ILoggerFactory _loggerFactory;

        public ExtensionsLogProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public ILog GetLogger(string name)
        {
            return new ExtensionsLog(_loggerFactory.CreateLogger(name));
        }
    }
}
