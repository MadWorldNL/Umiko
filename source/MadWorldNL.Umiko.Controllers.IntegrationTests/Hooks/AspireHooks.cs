using System.Text;
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
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {CreateFakeJwtToken()}");
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
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {CreateFakeJwtToken()}");
        return client;
    }

    private static string CreateFakeJwtToken()
    {
        var header = Base64UrlEncode("""{"alg":"none","typ":"JWT"}""");
        var payload = Base64UrlEncode("""{"sub":"test-user","preferred_username":"test-user","aud":"UmikoApi"}""");
        return $"{header}.{payload}.";
    }

    private static string Base64UrlEncode(string input)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(input))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
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