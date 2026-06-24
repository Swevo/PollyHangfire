/// <summary>Dependency-injection extensions for <c>PollyHangfire</c>.</summary>
public static class PollyHangfireServiceCollectionExtensions
{
    /// <summary>
    /// Registers a singleton <see cref="ResiliencePipeline"/> and a transient
    /// <see cref="ResilientBackgroundJobClient"/> wrapping the <see cref="IBackgroundJobClient"/>
    /// already registered in the DI container (e.g., by <c>services.AddHangfire(...)</c>).
    /// </summary>
    public static IServiceCollection AddPollyHangfire(
        this IServiceCollection services,
        Action<ResiliencePipelineBuilder> configure)
    {
        var builder = new ResiliencePipelineBuilder();
        configure(builder);
        var pipeline = builder.Build();

        services.AddSingleton(pipeline);
        services.AddTransient<ResilientBackgroundJobClient>(sp =>
            sp.GetRequiredService<IBackgroundJobClient>().WithPolly(pipeline));

        return services;
    }
}
