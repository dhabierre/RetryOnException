namespace AnanApps.Strategies.RetryOnException.Tests
{
    using System;
    using System.Threading;

    interface IRetryOnExceptionService
    {
        void Execute(Action<CancellationToken> action, CancellationToken token);
    }
}
