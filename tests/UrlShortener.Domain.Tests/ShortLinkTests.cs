using UrlShortener.Domain.ShortLinks;

namespace UrlShortener.Domain.Tests;

public class ShortLinkTests
{
    [Fact]
    public void Create_WithValidInput_CreatesShortLink()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var expiresAt = createdAt.AddDays(7);

        var shortLink = ShortLink.Create("abc123", "https://example.com/page", createdAt, expiresAt);

        Assert.Equal("abc123", shortLink.Code);
        Assert.Equal("https://example.com/page", shortLink.OriginalUrl);
        Assert.Equal(createdAt, shortLink.CreatedAtUtc);
        Assert.Equal(expiresAt, shortLink.ExpiresAtUtc);
        Assert.Equal(0, shortLink.ClickCount);
    }

    [Fact]
    public void Create_WithInvalidUrl_ThrowsArgumentException()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var action = () => ShortLink.Create("abc123", "not-a-url", createdAt, null);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void IsExpired_WhenNowIsAfterExpiration_ReturnsTrue()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var expiresAt = createdAt.AddHours(1);
        var shortLink = ShortLink.Create("abc123", "https://example.com", createdAt, expiresAt);

        var isExpired = shortLink.IsExpired(createdAt.AddHours(2));

        Assert.True(isExpired);
    }

    [Fact]
    public void RegisterClick_WhenNotExpired_IncrementsClickCount()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var expiresAt = createdAt.AddDays(1);
        var shortLink = ShortLink.Create("abc123", "https://example.com", createdAt, expiresAt);

        shortLink.RegisterClick(createdAt.AddHours(1));

        Assert.Equal(1, shortLink.ClickCount);
    }

    [Fact]
    public void RegisterClick_WhenExpired_ThrowsInvalidOperationException()
    {
        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var expiresAt = createdAt.AddHours(1);
        var shortLink = ShortLink.Create("abc123", "https://example.com", createdAt, expiresAt);

        var action = () => shortLink.RegisterClick(createdAt.AddHours(2));

        Assert.Throws<InvalidOperationException>(action);
    }
}
