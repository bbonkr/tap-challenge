using System;
using System.Threading;
using System.Threading.Tasks;

namespace SampleTAP
{
    public class SampleProcess
    {
        public Task RunAsync(int delaySeconds, CancellationToken token = default(CancellationToken))
        {
            if (delaySeconds > 0)
            {
                Task.Delay(TimeSpan.FromSeconds(delaySeconds)).Wait();
            }

            if (token.IsCancellationRequested)
            {
                return Task.FromCanceled(token);
            }

            if (!token.IsCancellationRequested)
            {
                throw new Exception("Error on async task.");
            }

            return Task.CompletedTask;
        }

        public Task AfterWork(CancellationToken token = default(CancellationToken))
        {
            Console.WriteLine($"==> {nameof(AfterWork)}");

            return Task.CompletedTask;
        }

        public Task<string> RunAndReturnsAsync(CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult("Ok");
        }
    }
}
