namespace AnanApps.Strategies.RetryOnException.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using FluentAssertions;
    using NUnit.Framework;

    internal class FactorRetryOnExceptionTests
    {
        [Test]
        public void FactorRetryOnException_NoException_NoRetry()
        {
            var retryCounter = -1;

            var retryService = new TimeoutFactorRetryOnExceptionService(times: 3, delay: TimeSpan.FromMilliseconds(100), factor: 2);

            retryService.Execute((token) =>
            {
                retryCounter++;
            },
            CancellationToken.None);

            retryCounter.Should().Be(0);
        }

        [Test]
        public void FactorRetryOnException_OneTimeoutException_SuccessWithOneRetry()
        {
            var retryCounter = 0;
            var exceptionCounter = 0;

            var retryService = new TimeoutFactorRetryOnExceptionService(times: 3, delay: TimeSpan.FromMilliseconds(100), factor: 2);

            retryService.Execute((token) =>
            {
                if (retryCounter == 0) // throws TimeoutException on the first action call
                {
                    retryCounter++;
                    exceptionCounter++;

                    throw new TimeoutException("Timeout action!");
                }
            },
            CancellationToken.None);
        }

        [Test]
        public void FactorRetryOnException_ManyTimeoutExceptions_SuccessWithManyRetries()
        {
            var retryCounter = 0;
            var exceptionCounter = 0;

            var retryService = new TimeoutFactorRetryOnExceptionService(times: 3, delay: TimeSpan.FromMilliseconds(100), factor: 2);

            retryService.Execute((token) =>
            {
                if (retryCounter == 0 || retryCounter == 1) // throws TimeoutException on the first and second action calls
                {
                    retryCounter++;
                    exceptionCounter++;

                    throw new TimeoutException("Timeout action!");
                }
            },
            CancellationToken.None);

            exceptionCounter.Should().Be(2);
            retryCounter.Should().Be(2);
        }

        [Test]
        public void FactorRetryOnException_AlwaysTimeoutExceptions_ThrowsException()
        {
            var exceptionCounter = 0;

            Action throws = () =>
            {
                var retryService = new TimeoutFactorRetryOnExceptionService(times: 3, delay: TimeSpan.FromMilliseconds(100), factor: 2);

                retryService.Execute((token) =>
                {
                    exceptionCounter++;

                    throw new TimeoutException("Timeout action!");
                },
                CancellationToken.None);
            };

            throws.Should().Throw<TimeoutException>();
            exceptionCounter.Should().Be(3);
        }

        [Test]
        public void FactorRetryOnException_UnmanagedExceptions_ThrowsException()
        {
            var exceptionCounter = 0;

            Action throws = () =>
            {
                var retryService = new TimeoutFactorRetryOnExceptionService(times: 3, delay: TimeSpan.FromMilliseconds(100), factor: 2);

                retryService.Execute((token) =>
                {
                    exceptionCounter++;

                    throw new OperationCanceledException("Operation canceled action!");
                },
                CancellationToken.None);
            };

            throws.Should().Throw<OperationCanceledException>();
            exceptionCounter.Should().Be(1);
        }
    }
    
    internal class TimeoutFactorRetryOnExceptionService : IRetryOnExceptionService
    {
        private readonly IRetryOnExceptionPolicy policy;
        private readonly IEnumerable<Type> exceptions;

        public TimeoutFactorRetryOnExceptionService(int times, TimeSpan delay, int factor)
        {
            this.exceptions = new[] { typeof(TimeoutException) };
            this.policy = RetryOnExceptionPolicyFactory.CreateFactorRetryOnExceptionPolicy(times, delay, factor);
        }

        public void Execute(Action<CancellationToken> action, CancellationToken token)
        {
            var context = new RetryOnExceptionContext(this.policy, this.exceptions, token);

            RetryOnException.Action(context, action);
        }
    }
}