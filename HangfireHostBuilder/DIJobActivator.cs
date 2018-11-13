using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HangfireHostBuilder
{
    public class DIJobActivator : JobActivator
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DIJobActivator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public override JobActivatorScope BeginScope(JobActivatorContext context)
        {
            return new DIJobActivatorScope(_serviceScopeFactory.CreateScope());
        }

#pragma warning disable CS0672 // Member overrides obsolete member
        public override JobActivatorScope BeginScope()
#pragma warning restore CS0672 // Member overrides obsolete member
        {
            return new DIJobActivatorScope(_serviceScopeFactory.CreateScope());
        }
    }
}
