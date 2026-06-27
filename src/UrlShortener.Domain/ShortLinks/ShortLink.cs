namespace UrlShortener.Domain.ShortLinks;

public sealed class ShortLink
{
    private ShortLink()
    {
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string OriginalUrl { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset? ExpiresAtUtc { get; private set; }

    public long ClickCount { get; private set; }

    public static ShortLink Create(
        string code,
        string originalUrl,
        DateTimeOffset createdAtUtc,
        DateTimeOffset? expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Code is required.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(originalUrl))
        {
            throw new ArgumentException("Original URL is required.", nameof(originalUrl));
        }

        if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out _))
        {
            throw new ArgumentException("Original URL must be a valid absolute URL.", nameof(originalUrl));
        }

        if (expiresAtUtc.HasValue && expiresAtUtc.Value <= createdAtUtc)
        {
            throw new ArgumentException("Expiration must be after creation time.", nameof(expiresAtUtc));
        }

        return new ShortLink
        {
            Id = Guid.NewGuid(),
            Code = code.Trim(),
            OriginalUrl = originalUrl.Trim(),
            CreatedAtUtc = createdAtUtc,
            ExpiresAtUtc = expiresAtUtc,
            ClickCount = 0
        };
    }

    public bool IsExpired(DateTimeOffset nowUtc)
    {
        return ExpiresAtUtc.HasValue && nowUtc >= ExpiresAtUtc.Value;
    }

    public void RegisterClick(DateTimeOffset nowUtc)
    {
        if (IsExpired(nowUtc))
        {
            throw new InvalidOperationException("Cannot register click for an expired short link.");
        }

        ClickCount++;
    }
}
