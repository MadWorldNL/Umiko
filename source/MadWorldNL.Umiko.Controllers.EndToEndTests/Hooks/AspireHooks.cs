using Aspire.Hosting;
using Microsoft.Extensions.Logging;

namespace MadWorldNL.Umiko.Hooks;

[Binding]
public class AspireHooks
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(600);

    private static DistributedApplication? _app;
    private static IPlaywright? _playwright;
    private static IBrowser? _browser;

    public static DistributedApplication App => _app ?? throw new InvalidOperationException("Aspire app not initialized");
    public static IBrowser Browser => _browser ?? throw new InvalidOperationException("Browser not initialized");

    public static Task<IBrowserContext> NewContextAsync() => Browser.NewContextAsync(new BrowserNewContextOptions
    {
        IgnoreHTTPSErrors = true
    });

    [BeforeTestRun]
    public static async Task BeforeTestRun()
    {
        var cancellationToken = CancellationToken.None;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Aspire_Tests>(cancellationToken);
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        _app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await _app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        if (_browser is not null)
        {
            await _browser.DisposeAsync();
        }

        _playwright?.Dispose();

        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }
}