/// <summary>
/// Pre-built Polly <see cref="PredicateBuilder"/> for transient Hangfire job client errors.
/// Covers job client failures, distributed lock timeouts, and cancellations.
/// </summary>
/// <remarks>
/// <b>Key differentiator:</b> Hangfire's built-in retry only applies to job <i>execution</i>
/// on the server side. This predicate is for the <i>client</i> side — when
/// <see cref="IBackgroundJobClient.Create"/> fails because the job store is temporarily unavailable.
/// </remarks>
public static class HangfireTransientErrors
{
    /// <summary>
    /// A <see cref="PredicateBuilder"/> that handles:
    /// <list type="bullet">
    ///   <item><see cref="BackgroundJobClientException"/> — job client failed to enqueue/schedule</item>
    ///   <item><see cref="DistributedLockTimeoutException"/> — distributed lock acquisition timed out</item>
    ///   <item><see cref="TimeoutException"/> — store connection or command timed out</item>
    ///   <item><see cref="TaskCanceledException"/> — operation cancelled due to timeout</item>
    /// </list>
    /// Assign to <c>ShouldHandle</c> on any Polly strategy.
    /// </summary>
    public static readonly PredicateBuilder IsTransient =
        (PredicateBuilder)new PredicateBuilder()
            .Handle<BackgroundJobClientException>()
            .Handle<DistributedLockTimeoutException>()
            .Handle<TimeoutException>()
            .Handle<TaskCanceledException>();
}
