using System.Security.Cryptography;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Infrastructure.Services;

public sealed class RandomShortCodeGenerator : IShortCodeGenerator
{
    private const int CodeLength = 7;
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public string GenerateCode()
    {
        Span<char> chars = stackalloc char[CodeLength];

        for (var i = 0; i < chars.Length; i++)
        {
            var index = RandomNumberGenerator.GetInt32(Alphabet.Length);
            chars[i] = Alphabet[index];
        }

        return new string(chars);
    }
}
