/// <summary>
/// Wraps an <see cref="IBackgroundJobClient"/> with a Polly v8 <see cref="ResiliencePipeline"/>,
/// applying retry and circuit-breaker to every enqueue and schedule operation.
/// </summary>
/// <remarks>
/// Hangfire's built-in retry applies to job <i>execution</i> only. This wrapper adds
/// client-side resilience so that transient job store failures (network blips, storage
/// restarts) do not cause enqueue calls to fail silently.
/// </remarks>
public sealed class ResilientBackgroundJobClient(IBackgroundJobClient client, ResiliencePipeline pipeline)
{
    /// <summary>The underlying <see cref="IBackgroundJobClient"/>.</summary>
    public IBackgroundJobClient Inner => client;

    /// <summary>Enqueues a fire-and-forget job, protected by the resilience pipeline.</summary>
    public string Enqueue(Expression<Action> methodCall)
        => pipeline.Execute(() => client.Enqueue(methodCall));

    /// <summary>Enqueues a fire-and-forget job for a specific type, protected by the resilience pipeline.</summary>
    public string Enqueue<T>(Expression<Action<T>> methodCall)
        => pipeline.Execute(() => client.Enqueue(methodCall));

    /// <summary>Schedules a delayed job, protected by the resilience pipeline.</summary>
    public string Schedule(Expression<Action> methodCall, TimeSpan delay)
        => pipeline.Execute(() => client.Schedule(methodCall, delay));

    /// <summary>Schedules a delayed job for a specific type, protected by the resilience pipeline.</summary>
    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
        => pipeline.Execute(() => client.Schedule(methodCall, delay));

    /// <summary>
    /// Executes any <see cref="IBackgroundJobClient"/> operation, protected by the resilience pipeline.
    /// </summary>
    public T Execute<T>(Func<IBackgroundJobClient, T> operation)
        => pipeline.Execute(() => operation(client));
}
