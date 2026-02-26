using System.Net.Http.Json;
using MadWorldNL.Umiko.CurriculaVitae;

namespace MadWorldNL.Umiko.StepDefinitions.Api.CurriculaVitae;

[Binding]
[Scope(Feature = "Api CurriculaVitae Endpoints")]
public sealed class CurriculaVitaeSteps
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(500);

    private readonly string _forwardedIp = AspireHooks.GenerateRandomIp();

    private HttpResponseMessage? _response;
    private Guid _createdId;

    [Given("the {word} service is healthy")]
    public static async Task GivenTheServiceIsHealthy(string serviceName)
    {
        var cancellationToken = CancellationToken.None;
        await AspireHooks.App.ResourceNotifications
            .WaitForResourceHealthyAsync(serviceName, cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
    }

    [When("I create a curriculum vitae with first name {string} and last name {string} on the {word} service")]
    public async Task WhenICreateACurriculumVitae(string firstName, string lastName, string serviceName)
    {
        using var httpClient = AspireHooks.CreateHttpClient(serviceName, _forwardedIp);
        _response = await httpClient.PostAsJsonAsync("/CurriculaVitae", new CreateCurriculumVitaeRequest
        {
            FirstName = firstName,
            LastName = lastName
        }, CancellationToken.None);

        _response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
        var body = await _response.Content.ReadFromJsonAsync<CreateCurriculumVitaeResponse>(CancellationToken.None);
        _createdId = body!.Id;
    }

    [When("I get a curriculum vitae with an unknown id on the {word} service")]
    public async Task WhenIGetACurriculumVitaeWithAnUnknownId(string serviceName)
    {
        using var httpClient = AspireHooks.CreateHttpClient(serviceName, _forwardedIp);
        _response = await httpClient.GetAsync($"/CurriculaVitae/{Guid.NewGuid()}", CancellationToken.None);
    }

    [Then("the response status code should be Accepted")]
    public void ThenTheResponseStatusCodeShouldBeAccepted()
    {
        _response.ShouldNotBeNull();
        _response!.StatusCode.ShouldBe(HttpStatusCode.Accepted);
    }

    [Then("the response status code should be NotFound")]
    public void ThenTheResponseStatusCodeShouldBeNotFound()
    {
        _response.ShouldNotBeNull();
        _response!.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Then("the curriculum vitae should eventually be retrievable on the {word} service")]
    public async Task ThenTheCurriculumVitaeShouldEventuallyBeRetrievable(string serviceName)
    {
        using var cts = new CancellationTokenSource(DefaultTimeout);

        while (!cts.Token.IsCancellationRequested)
        {
            using var httpClient = AspireHooks.CreateHttpClient(serviceName, AspireHooks.GenerateRandomIp());
            var response = await httpClient.GetAsync($"/CurriculaVitae/{_createdId}", cts.Token);
            if (response.StatusCode == HttpStatusCode.OK)
                return;

            await Task.Delay(PollInterval, cts.Token);
        }

        throw new TimeoutException($"CurriculumVitae {_createdId} was not retrievable within the timeout.");
    }
}