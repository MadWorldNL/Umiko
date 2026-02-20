namespace MadWorldNL.Umiko.StepDefinitions.Bus.RateLimiter;

[Binding]
[Scope(Feature = "Bus Rate Limiter")]
public class RateLimiterSteps
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

    private readonly string _forwardedIp = AspireHooks.GenerateRandomIp();
    private readonly List<HttpResponseMessage> _responses = [];

    [Given("the {word} service is healthy")]
    public async Task GivenTheServiceIsHealthy(string serviceName)
    {
        var cancellationToken = CancellationToken.None;
        await AspireHooks.App.ResourceNotifications
            .WaitForResourceHealthyAsync(serviceName, cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
    }

    [When("I send {int} GET requests to {string} on the {word} service")]
    public async Task WhenISendGetRequestsToOnTheService(int count, string path, string serviceName)
    {
        using var httpClient = AspireHooks.CreateRawHttpClient(serviceName, _forwardedIp);
        for (var i = 0; i < count; i++)
        {
            var response = await httpClient.GetAsync(path, CancellationToken.None);
            _responses.Add(response);
        }
    }

    [Then("all response status codes should be OK")]
    public void ThenAllResponseStatusCodesShouldBeOk()
    {
        _responses.ShouldNotBeEmpty();
        _responses.ShouldAllBe(response => response.StatusCode == HttpStatusCode.OK);
    }

    [Then("the first {int} response status codes should be OK")]
    public void ThenTheFirstResponseStatusCodesShouldBeOk(int count)
    {
        _responses.Count.ShouldBeGreaterThanOrEqualTo(count);
        _responses.Take(count).ShouldAllBe(response => response.StatusCode == HttpStatusCode.OK);
    }

    [Then("the last response status code should be TooManyRequests")]
    public void ThenTheLastResponseStatusCodeShouldBeTooManyRequests()
    {
        _responses.ShouldNotBeEmpty();
        _responses[^1].StatusCode.ShouldBe(HttpStatusCode.TooManyRequests);
    }
}
