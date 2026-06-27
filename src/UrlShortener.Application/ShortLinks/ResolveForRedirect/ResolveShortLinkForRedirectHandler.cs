using UrlShortener.Application.Abstractions;

namespace UrlShortener.Application.ShortLinks.ResolveForRedirect;

public sealed class ResolveShortLinkForRedirectHandler(IShortLinkRepository shortLinkRepository)
{
    public async Task<ResolveShortLinkForRedirectResult> HandleAsync(string code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Code is required.", nameof(code));
        }

        var normalizedCode = code.Trim();
        var shortLink = await shortLinkRepository.GetByCodeAsync(normalizedCode, cancellationToken);

        if (shortLink is null)
        {
            return ResolveShortLinkForRedirectResult.NotFound();
        }

        var nowUtc = DateTimeOffset.UtcNow;
        if (shortLink.IsExpired(nowUtc))
        {
            return ResolveShortLinkForRedirectResult.ExpiredResult();
        }

        shortLink.RegisterClick(nowUtc);
        await shortLinkRepository.SaveChangesAsync(cancellationToken);

        return ResolveShortLinkForRedirectResult.Success(shortLink.OriginalUrl);
    }
}
