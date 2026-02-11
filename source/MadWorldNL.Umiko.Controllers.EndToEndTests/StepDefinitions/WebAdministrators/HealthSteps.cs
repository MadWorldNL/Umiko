namespace MadWorldNL.Umiko.StepDefinitions.WebAdministrators;

[Binding]
[Scope(Feature = "Web Administrators Health")]
public class HealthSteps
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

    private IBrowserContext? _context;
    private IPage? _page;
    private IResponse? _response;
    private readonly List<string> _consoleErrors = [];

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        _context = await AspireHooks.NewContextAsync();
        _page = await _context.NewPageAsync();
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        if (_context is not null)
        {
            await _context.DisposeAsync();
        }
    }

    [Given("the {word} service is healthy")]
    public async Task GivenTheServiceIsHealthy(string serviceName)
    {
        var cancellationToken = CancellationToken.None;
        await AspireHooks.App.ResourceNotifications
            .WaitForResourceHealthyAsync(serviceName, cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
    }

    [When("I navigate to the health page on {word}")]
    public async Task WhenINavigateToTheHealthPageOn(string serviceName)
    {
        var endpoint = AspireHooks.App.GetEndpoint(serviceName, "https");
        _response = await _page!.GotoAsync($"{endpoint}health");
    }

    [When("I navigate to the home page on {word}")]
    public async Task WhenINavigateToTheHomePageOn(string serviceName)
    {
        var endpoint = AspireHooks.App.GetEndpoint(serviceName, "https");
        _response = await _page!.GotoAsync(endpoint.ToString());
    }

    [When("I navigate to the home page on {word} and wait for it to load")]
    public async Task WhenINavigateToTheHomePageOnAndWaitForItToLoad(string serviceName)
    {
        var endpoint = AspireHooks.App.GetEndpoint(serviceName, "https");

        _page!.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                _consoleErrors.Add(msg.Text);
            }
        };

        await _page.GotoAsync(endpoint.ToString());
        await _page.WaitForSelectorAsync("h1");
    }

    [Then("the page should return status code {int}")]
    public void ThenThePageShouldReturnStatusCode(int statusCode)
    {
        Assert.NotNull(_response);
        Assert.Equal(statusCode, _response!.Status);
    }

    [Then("there should be no console errors")]
    public void ThenThereShouldBeNoConsoleErrors()
    {
        Assert.Empty(_consoleErrors);
    }

    [Then("the heading should be {string}")]
    public async Task ThenTheHeadingShouldBe(string expectedHeading)
    {
        var heading = await _page!.Locator("h1").TextContentAsync();
        Assert.Equal(expectedHeading, heading);
    }
}