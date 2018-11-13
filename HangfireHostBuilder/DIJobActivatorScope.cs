using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConsoleApp5
{
    internal class DIJobActivatorScope : JobActivatorScope
    {
        private readonly IServiceScope _serviceScope;

        public DIJobActivatorScope(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope ?? throw new ArgumentNullException(nameof(serviceScope));
        }

        public override object Resolve(Type type)
        {
            return ActivatorUtilities.GetServiceOrCreateInstance(_serviceScope.ServiceProvider, type);
        }

        public override void DisposeScope()
        {
            _serviceScope.Dispose();
        }
    }
}
