using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Domain.ShortLinks;
using UrlShortener.Infrastructure.Persistence;

namespace UrlShortener.Api.IntegrationTests;

public class ShortLinksEndpointsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public ShortLinksEndpointsTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Create_Then_Analytics_Then_Redirect_Works_EndToEnd()
    {
        await _factory.ResetDatabaseAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/short-links", new
        {
            originalUrl = "https://example.com/path",
            expiresAtUtc = (DateTimeOffset?)null
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        using var createJson = JsonDocument.Parse(await createResponse.Content.ReadAsStringAsync());
        var root = createJson.RootElement;

        var code = root.GetProperty("code").GetString();
        var shortUrl = root.GetProperty("shortUrl").GetString();

        Assert.False(string.IsNullOrWhiteSpace(code));
        Assert.False(string.IsNullOrWhiteSpace(shortUrl));

        var analyticsBeforeResponse = await _client.GetAsync($"/api/short-links/{code}");
        Assert.Equal(HttpStatusCode.OK, analyticsBeforeResponse.StatusCode);

        using var analyticsBeforeJson = JsonDocument.Parse(await analyticsBeforeResponse.Content.ReadAsStringAsync());
        Assert.Equal(0, analyticsBeforeJson.RootElement.GetProperty("clickCount").GetInt64());

        var redirectResponse = await _client.GetAsync($"/{code}");
        Assert.Equal(HttpStatusCode.Found, redirectResponse.StatusCode);
        Assert.NotNull(redirectResponse.Headers.Location);
        Assert.Equal("https://example.com/path", redirectResponse.Headers.Location!.ToString());

        var analyticsAfterResponse = await _client.GetAsync($"/api/short-links/{code}");
        Assert.Equal(HttpStatusCode.OK, analyticsAfterResponse.StatusCode);

        using var analyticsAfterJson = JsonDocument.Parse(await analyticsAfterResponse.Content.ReadAsStringAsync());
        Assert.Equal(1, analyticsAfterJson.RootElement.GetProperty("clickCount").GetInt64());
    }

    [Fact]
    public async Task GetAnalytics_And_Redirect_ReturnNotFound_ForMissingCode()
    {
        await _factory.ResetDatabaseAsync();

        var analyticsResponse = await _client.GetAsync("/api/short-links/missing1");
        var redirectResponse = await _client.GetAsync("/missing1");

        Assert.Equal(HttpStatusCode.NotFound, analyticsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, redirectResponse.StatusCode);
    }

    [Fact]
    public async Task Redirect_ReturnsGone_ForExpiredCode()
    {
        await _factory.ResetDatabaseAsync();

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var createdAtUtc = DateTimeOffset.UtcNow.AddHours(-2);
            var expiredAtUtc = DateTimeOffset.UtcNow.AddMinutes(-1);

            var shortLink = ShortLink.Create("expired1", "https://example.com/expired", createdAtUtc, expiredAtUtc);
            await dbContext.ShortLinks.AddAsync(shortLink);
            await dbContext.SaveChangesAsync();
        }

        var redirectResponse = await _client.GetAsync("/expired1");

        Assert.Equal(HttpStatusCode.Gone, redirectResponse.StatusCode);
    }
}
