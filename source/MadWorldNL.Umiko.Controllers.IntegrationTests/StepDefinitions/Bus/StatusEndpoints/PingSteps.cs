namespace MadWorldNL.Umiko.StepDefinitions.Bus.StatusEndpoints;

[Binding]
[Scope(Feature = "Bus Ping Endpoint")]
public class PingSteps
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

    private readonly string _forwardedIp = AspireHooks.GenerateRandomIp();
    
    private HttpResponseMessage? _response;

    [Given("the {word} service is healthy")]
    public async Task GivenTheServiceIsHealthy(string serviceName)
    {
        var cancellationToken = CancellationToken.None;
        await AspireHooks.App.ResourceNotifications
            .WaitForResourceHealthyAsync(serviceName, cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
    }

    [When("I send a GET request to {string} on the {word} service")]
    public async Task WhenISendAGetRequestToOnTheService(string path, string serviceName)
    {
        using var httpClient = AspireHooks.CreateHttpClient(serviceName, _forwardedIp);
        _response = await httpClient.GetAsync(path, CancellationToken.None);
    }

    [Then("the response status code should be OK")]
    public void ThenTheResponseStatusCodeShouldBeOk()
    {
        Assert.NotNull(_response);
        Assert.Equal(HttpStatusCode.OK, _response!.StatusCode);
    }
}