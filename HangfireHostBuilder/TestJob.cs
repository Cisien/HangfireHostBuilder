using Hangfire;
using System;
using System.Threading.Tasks;

namespace ConsoleApp5
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
