namespace UrlShortener.Application.ShortLinks.CreateShortLink;

public sealed record CreateShortLinkResult(
    Guid Id,
    string Code,
    string OriginalUrl,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ExpiresAtUtc);
