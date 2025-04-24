using Reqnroll;
using Testcontainers.Redis;

namespace Integration;

[Binding]
public class TestRunBinding(ScenarioContext scenarioContext)
{
    private static HttpClient WebApplicationClient { get; set; } = null!;
    private RedisContainer RedisContainer { get; set; } = null!;

    [BeforeScenario]
    public async Task Before()
    {
        RedisContainer = new RedisBuilder()
            .WithImage("redis:7.0")
            .Build();
        await RedisContainer.StartAsync();
        var factory = new CustomWebApplicationFactory(RedisContainer);
        WebApplicationClient = factory.CreateClient();
        scenarioContext["WebApplicationClient"]=WebApplicationClient;
    }

    [AfterScenario]
    public async Task After()
    {
        await RedisContainer.DisposeAsync();
    }
}