public class PollyHangfireExtensionsTests
{
    private static readonly IBackgroundJobClient _client = Substitute.For<IBackgroundJobClient>();
    private static readonly ResiliencePipeline _pipeline = new ResiliencePipelineBuilder().Build();

    [Fact]
    public void WithPolly_Pipeline_ReturnsResilientBackgroundJobClient()
    {
        var resilient = _client.WithPolly(_pipeline);
        Assert.NotNull(resilient);
        Assert.Same(_client, resilient.Inner);
    }

    [Fact]
    public void WithPolly_Configure_ReturnsResilientBackgroundJobClient()
    {
        var resilient = _client.WithPolly(p => p.AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 3, Delay = TimeSpan.Zero,
            ShouldHandle = HangfireTransientErrors.IsTransient,
        }));
        Assert.NotNull(resilient);
        Assert.Same(_client, resilient.Inner);
    }
}
