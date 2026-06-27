using UrlShortener.Application.Abstractions;
using UrlShortener.Application.ShortLinks.GetAnalytics;
using UrlShortener.Domain.ShortLinks;

namespace UrlShortener.Application.Tests;

public class GetShortLinkAnalyticsHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenCodeNotFound_ReturnsNull()
    {
        var handler = new GetShortLinkAnalyticsHandler(new FakeShortLinkRepository());

        var result = await handler.HandleAsync("missing", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task HandleAsync_WhenCodeExists_ReturnsAnalytics()
    {
        var createdAtUtc = DateTimeOffset.UtcNow.AddHours(-2);
        var link = ShortLink.Create("abc123", "https://example.com", createdAtUtc, DateTimeOffset.UtcNow.AddHours(2));
        link.RegisterClick(DateTimeOffset.UtcNow.AddHours(-1));

        var repository = new FakeShortLinkRepository();
        repository.Add(link);

        var handler = new GetShortLinkAnalyticsHandler(repository);

        var result = await handler.HandleAsync("abc123", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(link.Id, result.Id);
        Assert.Equal("abc123", result.Code);
        Assert.Equal("https://example.com", result.OriginalUrl);
        Assert.Equal(createdAtUtc, result.CreatedAtUtc);
        Assert.Equal(link.ExpiresAtUtc, result.ExpiresAtUtc);
        Assert.Equal(1, result.ClickCount);
        Assert.False(result.IsExpired);
    }

    [Fact]
    public async Task HandleAsync_WhenCodeExpired_ReturnsExpiredTrue()
    {
        var createdAtUtc = DateTimeOffset.UtcNow.AddHours(-3);
        var link = ShortLink.Create("abc123", "https://example.com", createdAtUtc, DateTimeOffset.UtcNow.AddMinutes(-30));

        var repository = new FakeShortLinkRepository();
        repository.Add(link);

        var handler = new GetShortLinkAnalyticsHandler(repository);

        var result = await handler.HandleAsync("abc123", CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsExpired);
    }

    private sealed class FakeShortLinkRepository : IShortLinkRepository
    {
        private readonly Dictionary<string, ShortLink> _links = [];

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken)
        {
            return Task.FromResult(_links.ContainsKey(code));
        }

        public Task<ShortLink?> GetByCodeAsync(string code, CancellationToken cancellationToken)
        {
            _links.TryGetValue(code, out var shortLink);
            return Task.FromResult(shortLink);
        }

        public Task AddAsync(ShortLink shortLink, CancellationToken cancellationToken)
        {
            _links[shortLink.Code] = shortLink;
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Add(ShortLink shortLink)
        {
            _links[shortLink.Code] = shortLink;
        }
    }
}
