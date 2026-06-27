using UrlShortener.Application.Abstractions;
using UrlShortener.Application.ShortLinks.CreateShortLink;
using UrlShortener.Domain.ShortLinks;

namespace UrlShortener.Application.Tests;

public class CreateShortLinkHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithValidCommand_CreatesAndReturnsShortLink()
    {
        var generator = new FakeShortCodeGenerator(["abc123"]);
        var repository = new FakeShortLinkRepository();
        var handler = new CreateShortLinkHandler(repository, generator);

        var before = DateTimeOffset.UtcNow;

        var result = await handler.HandleAsync(
            new CreateShortLinkCommand("https://example.com", null),
            CancellationToken.None);

        var after = DateTimeOffset.UtcNow;

        Assert.Equal("abc123", result.Code);
        Assert.Equal("https://example.com", result.OriginalUrl);
        Assert.True(result.CreatedAtUtc >= before && result.CreatedAtUtc <= after);
        Assert.Single(repository.StoredItems);
    }

    [Fact]
    public async Task HandleAsync_WhenFirstCodeCollides_RetriesUntilUniqueCode()
    {
        var generator = new FakeShortCodeGenerator(["dup001", "ok002"]);
        var repository = new FakeShortLinkRepository(existingCodes: ["dup001"]);
        var handler = new CreateShortLinkHandler(repository, generator);

        var result = await handler.HandleAsync(
            new CreateShortLinkCommand("https://example.com/path", null),
            CancellationToken.None);

        Assert.Equal("ok002", result.Code);
        Assert.Equal(2, repository.CodeExistChecks);
        Assert.Single(repository.StoredItems);
    }

    [Fact]
    public async Task HandleAsync_WhenOriginalUrlIsMissing_ThrowsArgumentException()
    {
        var handler = new CreateShortLinkHandler(
            new FakeShortLinkRepository(),
            new FakeShortCodeGenerator(["abc123"]));

        var action = () => handler.HandleAsync(
            new CreateShortLinkCommand("", null),
            CancellationToken.None);

        await Assert.ThrowsAsync<ArgumentException>(action);
    }

    [Fact]
    public async Task HandleAsync_WhenNoUniqueCodeIsGenerated_ThrowsInvalidOperationException()
    {
        var generator = new FakeShortCodeGenerator(["dup", "dup", "dup", "dup", "dup"]);
        var repository = new FakeShortLinkRepository(existingCodes: ["dup"]);
        var handler = new CreateShortLinkHandler(repository, generator);

        var action = () => handler.HandleAsync(
            new CreateShortLinkCommand("https://example.com", null),
            CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }

    private sealed class FakeShortCodeGenerator(IEnumerable<string> values) : IShortCodeGenerator
    {
        private readonly Queue<string> _values = new(values);

        public string GenerateCode()
        {
            if (_values.Count == 0)
            {
                return "fallback";
            }

            return _values.Dequeue();
        }
    }

    private sealed class FakeShortLinkRepository(IEnumerable<string>? existingCodes = null) : IShortLinkRepository
    {
        private readonly HashSet<string> _existingCodes = new(existingCodes ?? []);

        public List<ShortLink> StoredItems { get; } = [];

        public int CodeExistChecks { get; private set; }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken)
        {
            CodeExistChecks++;
            return Task.FromResult(_existingCodes.Contains(code));
        }

        public Task AddAsync(ShortLink shortLink, CancellationToken cancellationToken)
        {
            StoredItems.Add(shortLink);
            _existingCodes.Add(shortLink.Code);
            return Task.CompletedTask;
        }

        public Task<ShortLink?> GetByCodeAsync(string code, CancellationToken cancellationToken)
        {
            var item = StoredItems.FirstOrDefault(shortLink => shortLink.Code == code);
            return Task.FromResult(item);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
