namespace AnanApps.Strategies.RetryOnException
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net;

    public static class RetryOnException
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RetryOnException));

        public static void Action(RetryOnExceptionContext context, Action<CancellationToken> operation)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            var attempts = 0;

            do
            {
                try
                {
                    attempts++;

                    operation(context.Token);

                    break;
                }
                catch (Exception e)
                {
                    if (!IsTransient(context, e))
                    {
                        throw;
                    }

                    if (attempts == context.Policy.Times)
                    {
                        throw;
                    }

                    var delay = context.Policy.GetDelay(attempts);

                    Log.Warn($"Exception on attempt {attempts} of {context.Policy.Times}. Will retry after {delay}", e);

                    Task.Delay(delay, context.Token).Wait(context.Token);
                }
            }
            while (true);
        }

        public static async Task ActionAsync(RetryOnExceptionContext context, Func<Task> operation)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            var attempts = 0;

            do
            {
                if (!context.Token.IsCancellationRequested)
                {
                    try
                    {
                        attempts++;

                        await operation();

                        break;
                    }
                    catch (Exception e)
                    {
                        if (!IsTransient(context, e))
                        {
                            throw;
                        }

                        if (attempts == context.Policy.Times)
                        {
                            throw;
                        }

                        var delay = context.Policy.GetDelay(attempts);

                        Log.Warn($"Exception on attempt {attempts} of {context.Policy.Times}. Will retry after {delay}", e);

                        await Task.Delay(delay, context.Token);
                    }
                }
            }
            while (true);
        }

        private static bool IsTransient(RetryOnExceptionContext context, Exception exception)
        {
            if (context.TransientExceptionTypes == null)
            {
                return false;
            }

            return context.TransientExceptionTypes.Contains(exception.GetType());
        }
    }
}