namespace MadWorldNL.Umiko.Api.StatusEndpoints;

[Collection(AspireCollection.Name)]
public class PingTests(AspireFixture fixture)
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

    [Fact]
    public async Task Ping_WhenCalled_ReturnsOkStatusCode()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        using var httpClient = fixture.App.CreateHttpClient("Api");
        await fixture.App.ResourceNotifications
            .WaitForResourceHealthyAsync("Api", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        // Act
        using var response = await httpClient.GetAsync("/Status/Ping", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}