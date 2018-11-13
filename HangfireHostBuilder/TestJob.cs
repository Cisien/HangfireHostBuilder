using Hangfire;
using System;
using System.Threading.Tasks;

namespace HangfireHostBuilder
{
    public class TestJob
    {

        public Task Execute(JobActivatorContext context)
        {
            Console.WriteLine(DateTimeOffset.UtcNow);
            return Task.CompletedTask;
        }
    }
}
