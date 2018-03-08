namespace AnanApps.Strategies.RetryOnException.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NUnit.Framework;

    internal class LinearRetryOnExceptionTests
    {
        [Test]
        public void LinearRetryOnException_NoException_NoRetry()
        {
            var retryCounter = -1;

            var retryService = new TimeoutLinearRetryOnExceptionService(times: 3, delay: TimeSpan.FromMilliseconds(100));

            retryService.Execute((token) =>
            {
                retryCounter++;
            },
            CancellationToken.None);

            retryCounter.Should().Be(0);
        }

        [Test]
        public void LinearRetryOnException_OneTimeoutException_SuccessWithOneRetry()
        {
            var retryCounter = 0;
            var exceptionCounter = 0;

            var retryService = new TimeoutLinearRetryOnExceptionService(times: 3, delay: TimeSpan.FromMilliseconds(100));

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
        public void LinearRetryOnException_ManyTimeoutExceptions_SuccessWithManyRetries()
        {
            var retryCounter = 0;
            var exceptionCounter = 0;

            var retryService = new TimeoutLinearRetryOnExceptionService(times: 3, delay: TimeSpan.FromMilliseconds(100));

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
        public void LinearRetryOnException_AlwaysTimeoutExceptions_ThrowsException()
        {
            var exceptionCounter = 0;

            Action throws = () =>
            {
                var retryService = new TimeoutLinearRetryOnExceptionService(times: 3, delay: TimeSpan.FromMilliseconds(100));

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
        public void LinearRetryOnException_UnmanagedExceptions_ThrowsException()
        {
            var exceptionCounter = 0;

            Action throws = () =>
            {
                var retryService = new TimeoutLinearRetryOnExceptionService(times: 3, delay: TimeSpan.FromMilliseconds(100));

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
    
    internal class TimeoutLinearRetryOnExceptionService : IRetryOnExceptionService
    {
        private readonly IRetryOnExceptionPolicy policy;
        private readonly IEnumerable<Type> transientExceptionTypes;

        public TimeoutLinearRetryOnExceptionService(int times, TimeSpan delay)
        {
            this.transientExceptionTypes = new[] { typeof(TimeoutException) };
            this.policy = RetryOnExceptionPolicyFactory.CreateLinearRetryOnExceptionPolicy(times, delay);
        }

        public void Execute(Action<CancellationToken> action, CancellationToken token)
        {
            var context = new RetryOnExceptionContext(this.policy, this.transientExceptionTypes, token);

            RetryOnException.Action(context, action);
        }

        public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken token)
        {
            var context = new RetryOnExceptionContext(this.policy, this.transientExceptionTypes, token);

            await RetryOnException.ActionAsync(context, action);
        }
    }
}