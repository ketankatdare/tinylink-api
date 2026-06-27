using UrlShortener.Application.Abstractions;
using UrlShortener.Application.ShortLinks.ResolveForRedirect;
using UrlShortener.Domain.ShortLinks;

namespace UrlShortener.Application.Tests;

public class ResolveShortLinkForRedirectHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenCodeDoesNotExist_ReturnsNotFound()
    {
        var repository = new FakeShortLinkRepository();
        var handler = new ResolveShortLinkForRedirectHandler(repository, new FakeClock(DateTimeOffset.UtcNow));

        var result = await handler.HandleAsync("missing", CancellationToken.None);

        Assert.False(result.Found);
        Assert.False(result.Expired);
        Assert.Null(result.OriginalUrl);
        Assert.Equal(0, repository.SaveChangesCalls);
    }

    [Fact]
    public async Task HandleAsync_WhenShortLinkIsExpired_ReturnsExpired()
    {
        var createdAtUtc = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var repository = new FakeShortLinkRepository();
        repository.Add(ShortLink.Create("abc123", "https://example.com", createdAtUtc, createdAtUtc.AddMinutes(30)));

        var handler = new ResolveShortLinkForRedirectHandler(
            repository,
            new FakeClock(createdAtUtc.AddHours(1)));

        var result = await handler.HandleAsync("abc123", CancellationToken.None);

        Assert.True(result.Found);
        Assert.True(result.Expired);
        Assert.Null(result.OriginalUrl);
        Assert.Equal(0, repository.SaveChangesCalls);
    }

    [Fact]
    public async Task HandleAsync_WhenShortLinkIsActive_ReturnsSuccessAndIncrementsClickCount()
    {
        var createdAtUtc = new DateTimeOffset(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);
        var link = ShortLink.Create("abc123", "https://example.com", createdAtUtc, createdAtUtc.AddDays(1));

        var repository = new FakeShortLinkRepository();
        repository.Add(link);

        var handler = new ResolveShortLinkForRedirectHandler(
            repository,
            new FakeClock(createdAtUtc.AddHours(1)));

        var result = await handler.HandleAsync("abc123", CancellationToken.None);

        Assert.True(result.Found);
        Assert.False(result.Expired);
        Assert.Equal("https://example.com", result.OriginalUrl);
        Assert.Equal(1, link.ClickCount);
        Assert.Equal(1, repository.SaveChangesCalls);
    }

    private sealed class FakeClock(DateTimeOffset utcNow) : IClock
    {
        public DateTimeOffset UtcNow { get; } = utcNow;
    }

    private sealed class FakeShortLinkRepository : IShortLinkRepository
    {
        private readonly Dictionary<string, ShortLink> _links = [];

        public int SaveChangesCalls { get; private set; }

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
            SaveChangesCalls++;
            return Task.CompletedTask;
        }

        public void Add(ShortLink shortLink)
        {
            _links[shortLink.Code] = shortLink;
        }
    }
}
