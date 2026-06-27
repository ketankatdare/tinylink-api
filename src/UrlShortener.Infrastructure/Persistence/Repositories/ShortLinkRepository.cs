using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.ShortLinks;

namespace UrlShortener.Infrastructure.Persistence.Repositories;

public sealed class ShortLinkRepository(AppDbContext dbContext) : IShortLinkRepository
{
    public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Task.FromResult(false);
        }

        var normalizedCode = code.Trim();

        return dbContext.ShortLinks
            .AsNoTracking()
            .AnyAsync(shortLink => shortLink.Code == normalizedCode, cancellationToken);
    }

    public Task AddAsync(ShortLink shortLink, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(shortLink);

        return AddAndSaveAsync(shortLink, cancellationToken);
    }

    private async Task AddAndSaveAsync(ShortLink shortLink, CancellationToken cancellationToken)
    {
        await dbContext.ShortLinks.AddAsync(shortLink, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
