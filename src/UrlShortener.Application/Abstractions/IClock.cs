namespace UrlShortener.Application.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
