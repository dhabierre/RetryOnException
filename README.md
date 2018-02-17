# RetryOnException Strategy

A simple simple retry pattern implementation in C#

[More information here](https://docs.microsoft.com/en-us/azure/architecture/patterns/retry)

Two retry policies available:

- LinearRetryOnExceptionPolicy
- FactorRetryOnExceptionPolicy

## Usage

First create a Service interface

```
public interface IRetryOnExceptionService
{
    void Execute(Action<CancellationToken> action, CancellationToken token);
}
```

Then implement it

```
internal class TimeoutFactorRetryOnExceptionService : IRetryOnExceptionService
{
    private readonly IRetryOnExceptionPolicy policy;
    private readonly IEnumerable<Type> transientExceptionTypes;

    public TimeoutFactorRetryOnExceptionService(int times, TimeSpan delay, int factor)
    {
        this.exceptions = new[] { typeof(TimeoutException) }; // perform retry on these exception types
        this.policy = RetryOnExceptionPolicyFactory.CreateFactorRetryOnExceptionPolicy(times, delay, factor);
    }

    public void Execute(Action<CancellationToken> action, CancellationToken token)
    {
        var context = new RetryOnExceptionContext(this.policy, this.transientExceptionTypes, token);

        RetryOnException.Action(context, action);
    }
}
```

Usage

```
var retryService = new TimeoutFactorRetryOnExceptionService(times: 3, delay: TimeSpan.FromMilliseconds(100), factor: 2);

retryService.Execute((token) =>
{
    // action that can throws TimeoutException...
},
CancellationToken.None);
```

Note: some samples are available in the unit tests


## Requirements

- Visual Studio 2017
- .NET Framework 4.5