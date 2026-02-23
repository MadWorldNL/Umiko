using System.Text.Json;

namespace MadWorldNL.Umiko.StepDefinitions.Bus.StatusEndpoints;

[Binding]
[Scope(Feature = "Bus MessageBus Endpoint")]
public sealed class MessageBusSteps
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

    private readonly string _forwardedIp = AspireHooks.GenerateRandomIp();

    private HttpResponseMessage? _response;
    private string? _responseContent;

    [Given("the {word} service is healthy")]
    public static async Task GivenTheServiceIsHealthy(string serviceName)
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
        _responseContent = await _response.Content.ReadAsStringAsync();
    }

    [Then("the response should contain {string} with value {string}")]
    public void ThenTheResponseShouldContainWithValue(string propertyName, string expectedValue)
    {
        _responseContent.ShouldNotBeNull();
        using var document = JsonDocument.Parse(_responseContent!);
        var property = document.RootElement.GetProperty(propertyName);
        property.ToString().ToLowerInvariant().ShouldBe(expectedValue);
    }
}