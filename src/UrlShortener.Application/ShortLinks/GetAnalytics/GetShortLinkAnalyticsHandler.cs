using UrlShortener.Application.Abstractions;

namespace UrlShortener.Application.ShortLinks.GetAnalytics;

public sealed class GetShortLinkAnalyticsHandler(IShortLinkRepository shortLinkRepository)
{
    public async Task<GetShortLinkAnalyticsResult?> HandleAsync(string code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Code is required.", nameof(code));
        }

        var shortLink = await shortLinkRepository.GetByCodeAsync(code.Trim(), cancellationToken);

        if (shortLink is null)
        {
            return null;
        }

        return new GetShortLinkAnalyticsResult(
            shortLink.Id,
            shortLink.Code,
            shortLink.OriginalUrl,
            shortLink.CreatedAtUtc,
            shortLink.ExpiresAtUtc,
            shortLink.ClickCount,
            shortLink.IsExpired(DateTimeOffset.UtcNow));
    }
}
