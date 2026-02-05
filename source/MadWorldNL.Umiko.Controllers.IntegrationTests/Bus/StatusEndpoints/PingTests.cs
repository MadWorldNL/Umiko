namespace MadWorldNL.Umiko.Bus.StatusEndpoints;

[Collection(AspireCollection.Name)]
public class PingTests(AspireFixture fixture)
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        using var httpClient = fixture.App.CreateHttpClient("Bus");
        await fixture.App.ResourceNotifications
            .WaitForResourceHealthyAsync("Bus", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        // Act
        using var response = await httpClient.GetAsync("/Status/Ping", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}