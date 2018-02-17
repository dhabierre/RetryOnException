namespace AnanApps.Strategies.RetryOnException
{
    using System;

    internal class FactorRetryOnExceptionPolicy : RetryOnExceptionPolicyBase
    {
        private readonly int factor;

        public FactorRetryOnExceptionPolicy(int times, TimeSpan delay, int factor)
            : base(times, delay)
        {
            if (factor <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(factor), factor, $"Must be greater than 0");
            }

            this.factor = factor;
        }

        public override TimeSpan GetDelay(int attempt)
        {
            if (attempt == 1)
            {
                return base.delay;
            }

            return TimeSpan.FromMilliseconds(base.delay.TotalMilliseconds * (attempt - 1) * factor);
        }
    }
}