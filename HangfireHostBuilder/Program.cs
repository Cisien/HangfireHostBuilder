using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hangfire;
using Hangfire.MemoryStorage;

namespace ConsoleApp5
{
    internal class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
            .ConfigureHostConfiguration(config => { })
            .ConfigureAppConfiguration(config => { })
            .ConfigureLogging(builder => {
                builder.AddConsole();
            })
            .ConfigureServices((context, services) => {
                services.AddHangfire(o => {
                o.UseMemoryStorage(new MemoryStorageOptions {
                        CountersAggregateInterval = TimeSpan.FromSeconds(10),
                        JobExpirationCheckInterval = TimeSpan.FromMinutes(1)
                    });
                });

                services.AddScoped<TestJob>();
                services.AddSingleton(new BackgroundJobServerOptions());

                services.AddHostedService<HangfireServer>();
            })
            .UseConsoleLifetime()
            .Build();
            
            host.Run();
        }

    }
}
