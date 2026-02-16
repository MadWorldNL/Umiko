using Aspire.Hosting;
using Microsoft.Extensions.Logging;

namespace MadWorldNL.Umiko.Hooks;

[Binding]
public class AspireHooks
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(600);

    private static DistributedApplication? _app;

    public static DistributedApplication App => _app ?? throw new InvalidOperationException("Aspire app not initialized");

    public static string GenerateRandomIp()
    {
        return $"10.{Random.Shared.Next(0, 256)}.{Random.Shared.Next(0, 256)}.{Random.Shared.Next(1, 256)}";
    }

    public static HttpClient CreateHttpClient(string serviceName, string ipAddress)
    {
        var client = App.CreateHttpClient(serviceName);
        client.DefaultRequestHeaders.Add("X-Forwarded-For", ipAddress);
        return client;
    }

    public static HttpClient CreateRawHttpClient(string serviceName, string ipAddress)
    {
        var endpoint = App.GetEndpoint(serviceName, "https");
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        var client = new HttpClient(handler)
        {
            BaseAddress = endpoint
        };
        client.DefaultRequestHeaders.Add("X-Forwarded-For", ipAddress);
        return client;
    }
    
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
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }
}