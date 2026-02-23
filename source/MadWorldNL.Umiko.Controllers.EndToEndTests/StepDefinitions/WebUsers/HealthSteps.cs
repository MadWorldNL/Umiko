namespace MadWorldNL.Umiko.StepDefinitions.WebUsers;

[Binding]
[Scope(Feature = "Web Users Health")]
public sealed class HealthSteps
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
    public static async Task GivenTheServiceIsHealthy(string serviceName)
    {
        var cancellationToken = CancellationToken.None;
        await AspireHooks.App.ResourceNotifications
            .WaitForResourceHealthyAsync(serviceName, cancellationToken)
            .WaitAsync(AspireHooks.DefaultTimeout, cancellationToken);
    }

    [When("I navigate to the health page on {word}")]
    public async Task WhenINavigateToTheHealthPageOn(string serviceName)
    {
        var endpoint = AspireHooks.App.GetEndpoint(serviceName, "http");
        _response = await _page!.GotoAsync($"{endpoint}health", new PageGotoOptions { Timeout = AspireHooks.DefaultTimeoutMilliseconds });
    }

    [When("I navigate to the home page on {word}")]
    public async Task WhenINavigateToTheHomePageOn(string serviceName)
    {
        var endpoint = AspireHooks.App.GetEndpoint(serviceName, "http");
        _response = await _page!.GotoAsync(endpoint.ToString(), new PageGotoOptions { Timeout = AspireHooks.DefaultTimeoutMilliseconds });
    }

    [When("I navigate to the home page on {word} and wait for it to load")]
    public async Task WhenINavigateToTheHomePageOnAndWaitForItToLoad(string serviceName)
    {
        var endpoint = AspireHooks.App.GetEndpoint(serviceName, "http");

        _page!.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                _consoleErrors.Add(msg.Text);
            }
        };

        await _page.GotoAsync(endpoint.ToString(), new PageGotoOptions { Timeout = AspireHooks.DefaultTimeoutMilliseconds });
        await _page.WaitForSelectorAsync("h1", new PageWaitForSelectorOptions { Timeout = AspireHooks.DefaultTimeoutMilliseconds });
    }

    [Then("the page should return status code {int}")]
    public void ThenThePageShouldReturnStatusCode(int statusCode)
    {
        _response.ShouldNotBeNull();
        _response!.Status.ShouldBe(statusCode);
    }

    [Then("there should be no console errors")]
    public void ThenThereShouldBeNoConsoleErrors()
    {
        _consoleErrors.ShouldBeEmpty();
    }

    [Then("the heading should be {string}")]
    public async Task ThenTheHeadingShouldBe(string expectedHeading)
    {
        var heading = await _page!.Locator("h1").TextContentAsync(new LocatorTextContentOptions() { Timeout = AspireHooks.DefaultTimeoutMilliseconds});
        heading.ShouldBe(expectedHeading);
    }
}