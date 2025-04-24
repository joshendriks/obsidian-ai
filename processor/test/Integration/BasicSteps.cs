using Reqnroll;

namespace Integration;

[Binding]
public class BasicSteps
{
    private readonly HttpClient _httpClient;

    public BasicSteps(ScenarioContext scenarioContext)
    {
        _httpClient = (HttpClient)scenarioContext["WebApplicationClient"];
    }

    [Given("start the thing")]
    public async Task GivenStartTheThing()
    {
        var result = await _httpClient.GetAsync("/");
        Assert.Equal("\"Hello World!\"", await result.Content.ReadAsStringAsync());
    }
}