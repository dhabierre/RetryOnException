namespace AnanApps.Strategies.RetryOnException.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    interface IRetryOnExceptionService
    {
        void Execute(Action<CancellationToken> action, CancellationToken token);

        Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken token);
    }
}
