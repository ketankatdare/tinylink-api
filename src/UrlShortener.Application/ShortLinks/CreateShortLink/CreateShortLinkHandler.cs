using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.ShortLinks;

namespace UrlShortener.Application.ShortLinks.CreateShortLink;

public sealed class CreateShortLinkHandler
{
    private const int MaxCodeGenerationAttempts = 5;

    private readonly IShortLinkRepository _shortLinkRepository;
    private readonly IShortCodeGenerator _shortCodeGenerator;
    private readonly IClock _clock;

    public CreateShortLinkHandler(
        IShortLinkRepository shortLinkRepository,
        IShortCodeGenerator shortCodeGenerator,
        IClock clock)
    {
        _shortLinkRepository = shortLinkRepository;
        _shortCodeGenerator = shortCodeGenerator;
        _clock = clock;
    }

    public async Task<CreateShortLinkResult> HandleAsync(
        CreateShortLinkCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (string.IsNullOrWhiteSpace(command.OriginalUrl))
        {
            throw new ArgumentException("Original URL is required.", nameof(command));
        }

        var createdAtUtc = _clock.UtcNow;

        for (var attempt = 0; attempt < MaxCodeGenerationAttempts; attempt++)
        {
            var candidateCode = _shortCodeGenerator.GenerateCode();

            if (await _shortLinkRepository.CodeExistsAsync(candidateCode, cancellationToken))
            {
                continue;
            }

            var shortLink = ShortLink.Create(
                candidateCode,
                command.OriginalUrl,
                createdAtUtc,
                command.ExpiresAtUtc);

            await _shortLinkRepository.AddAsync(shortLink, cancellationToken);

            return new CreateShortLinkResult(
                shortLink.Id,
                shortLink.Code,
                shortLink.OriginalUrl,
                shortLink.CreatedAtUtc,
                shortLink.ExpiresAtUtc);
        }

        throw new InvalidOperationException("Failed to generate a unique short code.");
    }
}
