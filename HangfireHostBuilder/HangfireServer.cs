using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HangfireHostBuilder
{
    public class HangfireServer : IHostedService
    {
        private readonly BackgroundJobServerOptions _options;
        private readonly JobStorage _jobStorage;
        private readonly IEnumerable<IBackgroundProcess> _backgroundProcesses;
        private readonly IBackgroundJobFactory _jobFactory;
        private readonly IBackgroundJobPerformer _jobPerformer;
        private readonly IBackgroundJobStateChanger _jobStateChanger;
        private readonly IJobFilterProvider _jobFilterProvider;
        private readonly JobActivator _activator;
        private readonly ILogger<HangfireServer> _logger;
        private BackgroundJobServer _server;

        public HangfireServer(
            BackgroundJobServerOptions options,
            JobStorage jobStorage,
            IEnumerable<IBackgroundProcess> backgroundProcesses,
            IBackgroundJobFactory jobFactory,
            IBackgroundJobPerformer jobPerformer,
            IBackgroundJobStateChanger jobStateChanger,
            ILogger<HangfireServer> logger,
            IJobFilterProvider jobFilterProvider = default,
            JobActivator activator = default
        )
        {
            _options = options;
            _jobStorage = jobStorage;
            _backgroundProcesses = backgroundProcesses;
            _jobFactory = jobFactory;
            _jobPerformer = jobPerformer;
            _jobStateChanger = jobStateChanger;
            _jobFilterProvider = options.FilterProvider ?? jobFilterProvider;
            _activator = options.Activator ?? activator;
            
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _server = new BackgroundJobServer(_options, _jobStorage, _backgroundProcesses, _jobFilterProvider, _activator, _jobFactory, _jobPerformer, _jobStateChanger);
            _logger.LogInformation("Hangfire Started");
            
            RecurringJob.AddOrUpdate<TestJob>(job => job.Execute(null), Cron.Minutely);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _server.SendStop();
            _server.Dispose();
            _logger.LogInformation("Hangfire Stopped");
            return Task.CompletedTask;
        }
    }
}