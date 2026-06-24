public class PollyHangfireServiceCollectionExtensionsTests
{
    private static readonly IBackgroundJobClient _client = Substitute.For<IBackgroundJobClient>();

    [Fact]
    public void AddPollyHangfire_RegistersResiliencePipeline()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_client);
        services.AddPollyHangfire(p => { });
        Assert.NotNull(services.BuildServiceProvider().GetRequiredService<ResiliencePipeline>());
    }

    [Fact]
    public void AddPollyHangfire_RegistersResilientBackgroundJobClient()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_client);
        services.AddPollyHangfire(p => { });
        var resilient = services.BuildServiceProvider().GetRequiredService<ResilientBackgroundJobClient>();
        Assert.NotNull(resilient);
        Assert.Same(_client, resilient.Inner);
    }

    [Fact]
    public void AddPollyHangfire_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_client);
        Assert.Same(services, services.AddPollyHangfire(p => { }));
    }
}
