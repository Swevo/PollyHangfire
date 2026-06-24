public class HangfireTransientErrorsTests
{
    [Fact]
    public void IsTransient_IsNotNull()
        => Assert.NotNull(HangfireTransientErrors.IsTransient);

    [Fact]
    public void IsTransient_RetriesBackgroundJobClientException()
    {
        var pipeline = BuildRetryPipeline();
        var attempts = 0;
        Assert.Throws<BackgroundJobClientException>(() =>
            pipeline.Execute(() => { attempts++; throw new BackgroundJobClientException("failure", null!); }));
        Assert.Equal(2, attempts);
    }

    [Fact]
    public void IsTransient_RetriesDistributedLockTimeoutException()
    {
        var pipeline = BuildRetryPipeline();
        var attempts = 0;
        Assert.Throws<DistributedLockTimeoutException>(() =>
            pipeline.Execute(() => { attempts++; throw new DistributedLockTimeoutException("resource"); }));
        Assert.Equal(2, attempts);
    }

    [Fact]
    public void IsTransient_RetriesTimeoutException()
    {
        var pipeline = BuildRetryPipeline();
        var attempts = 0;
        Assert.Throws<TimeoutException>(() =>
            pipeline.Execute(() => { attempts++; throw new TimeoutException(); }));
        Assert.Equal(2, attempts);
    }

    [Fact]
    public void IsTransient_RetriesTaskCanceledException()
    {
        var pipeline = BuildRetryPipeline();
        var attempts = 0;
        Assert.Throws<TaskCanceledException>(() =>
            pipeline.Execute(() => { attempts++; throw new TaskCanceledException(); }));
        Assert.Equal(2, attempts);
    }

    [Fact]
    public void IsTransient_DoesNotRetryArgumentException()
    {
        var pipeline = BuildRetryPipeline();
        var attempts = 0;
        Assert.Throws<ArgumentException>(() =>
            pipeline.Execute(() => { attempts++; throw new ArgumentException("bad arg"); }));
        Assert.Equal(1, attempts);
    }

    private static ResiliencePipeline BuildRetryPipeline() =>
        new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 1, Delay = TimeSpan.Zero, ShouldHandle = HangfireTransientErrors.IsTransient })
            .Build();
}
