using UrlShortener.Domain.ShortLinks;

namespace UrlShortener.Application.Abstractions;

public interface IShortLinkRepository
{
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken);

    Task AddAsync(ShortLink shortLink, CancellationToken cancellationToken);
}
