namespace MadWorldNL.Umiko.StepDefinitions.Api.CurriculaVitae;

[Binding]
[Scope(Feature = "Api GetCurriculumVitae Endpoint")]
public sealed class GetCurriculumVitaeSteps
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

    private readonly string _forwardedIp = AspireHooks.GenerateRandomIp();

    private HttpResponseMessage? _response;

    [Given("the {word} service is healthy")]
    public static async Task GivenTheServiceIsHealthy(string serviceName)
    {
        var cancellationToken = CancellationToken.None;
        await AspireHooks.App.ResourceNotifications
            .WaitForResourceHealthyAsync(serviceName, cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
    }

    [When("I get a curriculum vitae with an unknown id on the {word} service")]
    public async Task WhenIGetACurriculumVitaeWithAnUnknownId(string serviceName)
    {
        using var httpClient = AspireHooks.CreateHttpClient(serviceName, _forwardedIp);
        _response = await httpClient.GetAsync($"/CurriculaVitae/{Guid.NewGuid()}", CancellationToken.None);
    }

    [Then("the response status code should be NotFound")]
    public void ThenTheResponseStatusCodeShouldBeNotFound()
    {
        _response.ShouldNotBeNull();
        _response!.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}