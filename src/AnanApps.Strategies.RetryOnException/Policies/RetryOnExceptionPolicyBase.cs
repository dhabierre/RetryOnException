namespace AnanApps.Strategies.RetryOnException
{
    using System;

    public abstract class RetryOnExceptionPolicyBase : IRetryOnExceptionPolicy
    {
        protected readonly TimeSpan delay;

        public RetryOnExceptionPolicyBase(int times, TimeSpan delay)
        {
            if (times <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(times), times, $"Must be greater than 0");
            }

            this.Times = times;
            this.delay = delay;
        }

        public int Times { get; }

        public abstract TimeSpan GetDelay(int attempt);
    }
}