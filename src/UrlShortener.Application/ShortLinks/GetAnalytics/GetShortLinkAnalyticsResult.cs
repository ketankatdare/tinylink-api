namespace UrlShortener.Application.ShortLinks.GetAnalytics;

public sealed record GetShortLinkAnalyticsResult(
    Guid Id,
    string Code,
    string OriginalUrl,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ExpiresAtUtc,
    long ClickCount,
    bool IsExpired);
