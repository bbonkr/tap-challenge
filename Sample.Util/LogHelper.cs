using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Util
{
    public class LogHelper
    {/// <summary>
     /// 예외 로깅 지원
     /// </summary>
     /// <param name="logger"></param>
     /// <param name="ex"></param>
        public static string LogException(Exception ex)
        {
            StringBuilder builder = new StringBuilder();

            LogException(builder, ex);

            var message = builder.ToString();

            // 예외에 대한 로깅은 내부에서 처리하므로 디버깅을 위한 로깅만 모아서 제공합니다.
            // ! 동일한 내용이 Error, Debug로 로깅됩니다. 의도된 동작입니다.
            Console.WriteLine(message);

            return message;
        }

        /// <summary>
        /// 정보 로깅 지원 
        /// <para>타입이름.멤버이름 :: 메시지</para>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="obj"></param>
        /// <param name="message"></param>
        /// <param name="memberName"></param>
        public static string LogOperation(object obj, string message, [CallerMemberName] string memberName = "")
        {
            var logMessage = $"{obj.GetType().GetTypeInfo().FullName}.{memberName} :: {message}";
            
            Console.WriteLine(logMessage);

            return logMessage;
        }

        private static void LogException(StringBuilder builder, Exception ex)
        {
            Console.WriteLine(ex.Message);

            builder.AppendLine($"{ex.GetType().GetTypeInfo().FullName} :: {ex.Message}");

            if (ex is AggregateException)
            {
                var aggregateException = ex as AggregateException;

                foreach (var e in aggregateException.InnerExceptions)
                {

                    builder.Append($"* ");

                    LogException(builder, e);
                }
            }

            if (ex is TaskCanceledException)
            {
                builder.AppendLine($"[!] 비동기 작업이 취소되었습니다. CancellationToken 또는 비동기 작업 내에서 발생하는 예외, ContinueWith 옵션을 확인하세요.");
            }

            if (ex is OperationCanceledException)
            {
                builder.AppendLine($"[!] 실행이 취소되었습니다. 실행 제한 시간과  CancellationToken 을 확인하세요.");
            }

            if (ex.InnerException != null)
            {
                builder.AppendLine($"[InnerException]");
                LogException(builder, ex.InnerException);
            }
        }
    }
}
