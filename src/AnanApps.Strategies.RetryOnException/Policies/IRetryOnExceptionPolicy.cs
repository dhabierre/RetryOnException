namespace AnanApps.Strategies.RetryOnException
{
    using System;

    public interface IRetryOnExceptionPolicy
    {
        int Times { get; }

        TimeSpan GetDelay(int attempt);
    }
}