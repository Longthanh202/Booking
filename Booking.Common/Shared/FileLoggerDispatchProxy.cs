using System.Reflection;

namespace Booking.Common.Shared
{
    public sealed class FileLoggerDispatchProxy<T> : DispatchProxy where T : class
    {
        private T _target = null!;

        public static T Create(T target)
        {
            var proxy = DispatchProxy.Create<T, FileLoggerDispatchProxy<T>>();
            ((FileLoggerDispatchProxy<T>)(object)proxy)._target = target;
            return proxy;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            try
            {
                var result = targetMethod!.Invoke(_target, args);

                if (result is Task task)
                {
                    return WrapTask(task, targetMethod.ReturnType);
                }

                return result;
            }
            catch (TargetInvocationException ex)
            {
                LogAndRethrow(ex.InnerException ?? ex);
                throw;
            }
            catch (Exception ex)
            {
                LogAndRethrow(ex);
                throw;
            }
        }

        private static object WrapTask(Task task, Type returnType)
        {
            if (returnType == typeof(Task))
            {
                return AwaitTask(task);
            }

            var resultType = returnType.GetGenericArguments()[0];
            return typeof(FileLoggerDispatchProxy<T>)
                .GetMethod(nameof(AwaitTaskWithResult), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(resultType)
                .Invoke(null, new object[] { task })!;
        }

        private static async Task AwaitTask(Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogAndRethrow(ex);
            }
        }

        private static async Task<TResult> AwaitTaskWithResult<TResult>(Task task)
        {
            try
            {
                return await ((Task<TResult>)task).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogAndRethrow(ex);
                throw;
            }
        }

        private static void LogAndRethrow(Exception ex)
        {
            FileLogger.Log(ex);
        }
    }
}