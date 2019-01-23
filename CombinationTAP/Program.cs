using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CombinationTAP
{
    class Program
    {
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            var ctsA = new CancellationTokenSource();
            var ctsB = new CancellationTokenSource();
            var ctsM = CancellationTokenSource.CreateLinkedTokenSource(
                cts.Token,
                ctsA.Token,
                ctsB.Token
                );

            var taskA = Task.Run(() =>
            {
                return new OperationA().RunAsync(ctsA.Token);
            }, ctsM.Token)
            .ContinueWith(t =>
            {
                Console.WriteLine(t.Result);
                Console.WriteLine($"Task {Task.CurrentId}: {t.Status}");

                //Console.WriteLine($"==[ Task {Task.CurrentId} ]==================================");
                //Console.WriteLine($"cts  : {cts.IsCancellationRequested}");
                //Console.WriteLine($"ctsA : {ctsA.IsCancellationRequested}");
                //Console.WriteLine($"ctsB : {ctsB.IsCancellationRequested}");
                //Console.WriteLine($"ctsM : {ctsM.IsCancellationRequested}");
            }, ctsM.Token);

            var taskB = Task.Run(() =>
            {
                return new OperationB().RunAsync(ctsB.Token);
            }, ctsM.Token)
            .ContinueWith(t =>
            {
                Console.WriteLine(t.Result);
                Console.WriteLine($"Task {Task.CurrentId}: {t.Status}");

                //Console.WriteLine($"==[ Task {Task.CurrentId} ]==================================");
                //Console.WriteLine($"cts  : {cts.IsCancellationRequested}");
                //Console.WriteLine($"ctsA : {ctsA.IsCancellationRequested}");
                //Console.WriteLine($"ctsB : {ctsB.IsCancellationRequested}");
                //Console.WriteLine($"ctsM : {ctsM.IsCancellationRequested}");
            }, ctsM.Token);

            try
            {
                Console.WriteLine($"Start Task: This message is on synchronous operation. @ {DateTimeOffset.UtcNow:mm:ss.fff}");

                //ctsA.CancelAfter(TimeSpan.FromSeconds(2));
                ctsM.CancelAfter(TimeSpan.FromSeconds(3));

                taskA.Wait(ctsM.Token);
                taskB.Wait(ctsM.Token);

                Console.WriteLine($"End Task:  This message is on synchronous operation. @ {DateTimeOffset.UtcNow:mm:ss.fff}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"4: ==> {ex.GetType().GetTypeInfo().FullName}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"4.1: ==> {ex.InnerException.GetType().GetTypeInfo().FullName}: {ex.InnerException.Message}");
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"3: ==> {ex.GetType().GetTypeInfo().FullName}: {ex.Message}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"3.1: ==> {ex.InnerException.GetType().GetTypeInfo().FullName}: {ex.InnerException.Message}");
                }
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine($"2: ==> {e.GetType().GetTypeInfo().FullName}::{e.Message}");
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"1: ==> {ex.GetType().GetTypeInfo().FullName}: : {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"==[ Main ]==================================");
                Console.WriteLine($"cts  : {cts.IsCancellationRequested}");
                Console.WriteLine($"ctsA : {ctsA.IsCancellationRequested}");
                Console.WriteLine($"ctsB : {ctsB.IsCancellationRequested}");
                Console.WriteLine($"ctsM : {ctsM.IsCancellationRequested}");

                Console.WriteLine("Press ENTER KEY to exit.");
                Console.ReadLine();
            }
        }
    }

    public class OperationA
    {
        public Task<string> RunAsync(CancellationToken token = default(CancellationToken))
        {
            Task.Delay(TimeSpan.FromSeconds(5)).Wait(token);

            if (token.IsCancellationRequested)
            {
                return Task.FromCanceled<string>(token);
            }

            return Task.FromResult($"Completed. ==> {GetType().GetTypeInfo()}.{nameof(RunAsync)}");
        }
    }

    public class OperationB
    {
        public Task<string> RunAsync(CancellationToken token = default(CancellationToken))
        {
            Task.Delay(TimeSpan.FromSeconds(5)).Wait(token);

            if (token.IsCancellationRequested)
            {
                return Task.FromCanceled<string>(token);
            }

            return Task.FromResult($"Completed. ==> {GetType().GetTypeInfo()}.{nameof(RunAsync)}");
        }
    }
}
