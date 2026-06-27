namespace UrlShortener.Application.ShortLinks.CreateShortLink;

public sealed record CreateShortLinkCommand(string OriginalUrl, DateTimeOffset? ExpiresAtUtc);
