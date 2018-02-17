namespace AnanApps.Strategies.RetryOnException
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class RetryOnExceptionContext
    {
        public RetryOnExceptionContext(IRetryOnExceptionPolicy policy, IEnumerable<Type> transientExceptionTypes, CancellationToken token)
        {
            this.Policy = policy ?? throw new ArgumentNullException(nameof(policy));
            this.TransientExceptionTypes = transientExceptionTypes;
            this.Token = token;
        }

        public CancellationToken Token { get; }

        public IRetryOnExceptionPolicy Policy { get; set; }

        public IEnumerable<Type> TransientExceptionTypes { get; set; }
    }
}