/// <summary>Extension methods for adding Polly resilience to Hangfire job clients.</summary>
public static class PollyHangfireExtensions
{
    /// <summary>Wraps an <see cref="IBackgroundJobClient"/> with the given <see cref="ResiliencePipeline"/>.</summary>
    public static ResilientBackgroundJobClient WithPolly(
        this IBackgroundJobClient client,
        ResiliencePipeline pipeline)
        => new(client, pipeline);

    /// <summary>Wraps an <see cref="IBackgroundJobClient"/> with a pipeline built by <paramref name="configure"/>.</summary>
    public static ResilientBackgroundJobClient WithPolly(
        this IBackgroundJobClient client,
        Action<ResiliencePipelineBuilder> configure)
    {
        var builder = new ResiliencePipelineBuilder();
        configure(builder);
        return new(client, builder.Build());
    }
}
