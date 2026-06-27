namespace UrlShortener.Application.ShortLinks.ResolveForRedirect;

public sealed record ResolveShortLinkForRedirectResult(bool Found, bool Expired, string? OriginalUrl)
{
    public static ResolveShortLinkForRedirectResult NotFound() => new(false, false, null);

    public static ResolveShortLinkForRedirectResult ExpiredResult() => new(true, true, null);

    public static ResolveShortLinkForRedirectResult Success(string originalUrl) => new(true, false, originalUrl);
}
