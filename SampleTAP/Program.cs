using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Sample.Util;

namespace SampleTAP
{
    class Program
    {
        static void Main(string[] args)
        {
            var delaySeconds = 0;
            var returnsValue = String.Empty;
            var sample = new SampleProcess();
            var cts = new CancellationTokenSource();
            if (delaySeconds > 3)
            {
                cts.CancelAfter(TimeSpan.FromSeconds(delaySeconds > 2 ? delaySeconds - 2 : delaySeconds));
            }

            var task = Task.Run(() =>
            {
                Console.WriteLine($"{Task.CurrentId}: Start task. @ {DateTimeOffset.UtcNow:mm:ss.fff}");
                return sample.RunAsync(delaySeconds, cts.Token);
            })
            //.ContinueWith(t => {
            //    LogHelper.LogException(t.Exception);
            //}, TaskContinuationOptions.OnlyOnFaulted)
            .ContinueWith(t =>
            {
                Console.WriteLine($"{Task.CurrentId}: and continue 1st. @ {DateTimeOffset.UtcNow:mm:ss.fff}");
                if (t.Exception != null)
                {
                    if (t.Exception is AggregateException)
                    {
                        foreach (var e in t.Exception.InnerExceptions)
                        {
                            Console.WriteLine($"Got one or more exceptions. {e.GetType().GetTypeInfo().FullName}::{e.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Got a exception. {t.Exception.GetType().GetTypeInfo().FullName}::{t.Exception.Message}");
                    }

                    //return Task.FromException(t.Exception);
                }
                else
                {
                    //return Task.CompletedTask;
                }
            }, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.NotOnFaulted)
            .ContinueWith(t =>
            {
                Console.WriteLine($"{Task.CurrentId}: and continue 2nd. @ {DateTimeOffset.UtcNow:mm:ss.fff}");
                Console.WriteLine("Call AfterWork.");

                return sample.AfterWork();
            }, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.NotOnFaulted)
            .ContinueWith(t =>
            {
                Console.WriteLine($"{Task.CurrentId}: and continue 3rd. @ {DateTimeOffset.UtcNow:mm:ss.fff}");
                Console.WriteLine("All done.");

                //return Task.CompletedTask;
            }, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.NotOnFaulted)
            .ContinueWith(t =>
            {
                Console.WriteLine($"{Task.CurrentId}: and continue 4th. @ {DateTimeOffset.UtcNow:mm:ss.fff}");
                return sample.RunAndReturnsAsync(cts.Token);
            }, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.NotOnFaulted)
            .ContinueWith(t => {
                Console.WriteLine($"{Task.CurrentId}: and continue 5th. @ {DateTimeOffset.UtcNow:mm:ss.fff}");
                returnsValue = t.Result.Result;
            }, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.NotOnFaulted);


            try
            {
                Console.WriteLine($"Start Task: This message is on synchronous operation. @ {DateTimeOffset.UtcNow:mm:ss.fff}");

                task.Wait(cts.Token);

                Console.WriteLine($"End Task:  This message is on synchronous operation. @ {DateTimeOffset.UtcNow:mm:ss.fff}");
            }
            catch (TaskCanceledException ex)
            {
                //Console.WriteLine($"4: ==> {ex.GetType().GetTypeInfo().FullName}: {ex.Message}");
                //if (ex.InnerException != null)
                //{
                //    Console.WriteLine($"4.1: ==> {ex.InnerException.GetType().GetTypeInfo().FullName}: {ex.InnerException.Message}");
                //}


                LogHelper.LogException(ex);
            }
            catch (OperationCanceledException ex)
            {
                //Console.WriteLine($"3: ==> {ex.GetType().GetTypeInfo().FullName}: {ex.Message}");

                //if (ex.InnerException != null)
                //{
                //    Console.WriteLine($"3.1: ==> {ex.InnerException.GetType().GetTypeInfo().FullName}: {ex.InnerException.Message}");
                //}


                LogHelper.LogException(ex);
            }
            catch (AggregateException ex)
            {
                //foreach (var e in ex.InnerExceptions)
                //{
                //    Console.WriteLine($"2: ==> {e.GetType().GetTypeInfo().FullName}::{e.Message}");
                //}

                LogHelper.LogException(ex);
            }
            catch (System.Exception ex)
            {
                //Console.WriteLine($"1: ==> {ex.GetType().GetTypeInfo().FullName}: : {ex.Message}");

                LogHelper.LogException(ex);
            }
            finally
            {
                Console.WriteLine($"Returns value: {returnsValue}");

                Console.WriteLine("Press ENTER KEY to exit.");
                Console.ReadLine();
            }
        }
    }
}
