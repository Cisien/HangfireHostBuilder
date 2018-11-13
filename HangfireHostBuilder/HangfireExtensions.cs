using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.Server;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace ConsoleApp5
{
    public static class HangfireExtensions
    {

        public static IServiceCollection AddHangfire(
                this IServiceCollection services,
                Action<IGlobalConfiguration> configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            // ===== Configurable services =====
            configuration(GlobalConfiguration.Configuration);
            services.TryAddSingleton(JobStorage.Current);
            services.TryAddSingleton(JobActivator.Current);
            services.TryAddSingleton<IJobFilterProvider>(JobFilterProviders.Providers);


            // ===== Internal services =====

            // NOTE: these are not required to be checked, because they only depend on already checked configurables,
            //       are not accessed directly, and can't be affected by customizations made from configuration block.

            services.TryAddSingleton<IBackgroundJobFactory, BackgroundJobFactory>();

            services.TryAddSingleton<IBackgroundJobStateChanger, BackgroundJobStateChanger>();

            services.TryAddSingleton<IBackgroundJobPerformer, BackgroundJobPerformer>();


            // ===== Client services =====

            // NOTE: these, on the other hand, need to be double-checked to be sure configuration block was executed, 
            //       in case of a client-only scenario with all configurables above replaced with custom implementations.

            services.TryAddSingleton<IBackgroundJobClient, BackgroundJobClient>();

            services.TryAddSingleton<IRecurringJobManager, RecurringJobManager>();


            // IGlobalConfiguration serves as a marker indicating that Hangfire's services 
            // were added to the service container (checked by IApplicationBuilder extensions).
            // 
            // Being a singleton, it also guarantees that the configuration callback will be 
            // executed just once upon initialization, so there's no need to double-check that.
            // 
            // It should never be replaced by another implementation !!!
            // AddSingleton() will throw an exception if it was already registered

            services.AddSingleton(serviceProvider =>
            {
                var configurationInstance = GlobalConfiguration.Configuration;

                // init defaults for log provider and job activator
                // they may be overwritten by the configuration callback later

                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                if (loggerFactory != null)
                {
                    configurationInstance.UseLogProvider(new ExtensionsLogProvider(loggerFactory));
                }

                var scopeFactory = serviceProvider.GetService<IServiceScopeFactory>();
                if (scopeFactory != null)
                {
                    configurationInstance.UseActivator(new DIJobActivator(scopeFactory));
                }

                // do configuration inside callback

                configuration(configurationInstance);

                return configurationInstance;
            });

            return services;
        }
    }
}
