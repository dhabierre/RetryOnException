namespace AnanApps.Strategies.RetryOnException
{
    using System;

    internal class LinearRetryOnExceptionPolicy : RetryOnExceptionPolicyBase
    {
        public LinearRetryOnExceptionPolicy(int times, TimeSpan delay)
            : base(times, delay)
        {
        }

        public override TimeSpan GetDelay(int attempt) => base.delay;
    }
}