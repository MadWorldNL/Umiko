namespace MadWorldNL.Umiko.WebAdministrators;

[Collection(AspireCollection.Name)]
public class HealthTests(AspireFixture fixture)
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

    [Fact]
    public async Task HealthPage_ReturnsSuccessfully()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await fixture.App.ResourceNotifications
            .WaitForResourceHealthyAsync("Web-Administrators", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        var endpoint = fixture.App.GetEndpoint("Web-Administrators", "https");
        await using var context = await fixture.NewContextAsync();
        var page = await context.NewPageAsync();

        // Act
        var response = await page.GotoAsync($"{endpoint}health");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(200, response.Status);
    }

    [Fact]
    public async Task HomePage_LoadsSuccessfully()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        await fixture.App.ResourceNotifications
            .WaitForResourceHealthyAsync("Web-Administrators", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        var endpoint = fixture.App.GetEndpoint("Web-Administrators", "https");
        await using var context = await fixture.NewContextAsync();
        var page = await context.NewPageAsync();

        // Act
        var response = await page.GotoAsync(endpoint.ToString());

        // Assert
        Assert.NotNull(response);
        Assert.Equal(200, response.Status);
    }
}