namespace AnanApps.Strategies.RetryOnException
{
    using System;

    public static class RetryOnExceptionPolicyFactory
    {
        public static IRetryOnExceptionPolicy CreateLinearRetryOnExceptionPolicy(int times, TimeSpan delay)
        {
            return new LinearRetryOnExceptionPolicy(times, delay);
        }

        public static IRetryOnExceptionPolicy CreateFactorRetryOnExceptionPolicy(int times, TimeSpan delay, int factor)
        {
            return new FactorRetryOnExceptionPolicy(times, delay, factor);
        }
    }
}