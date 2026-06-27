using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.ShortLinks.CreateShortLink;
using UrlShortener.Application.ShortLinks.GetAnalytics;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("api/short-links")]
public sealed class ShortLinksController(
    CreateShortLinkHandler createShortLinkHandler,
    GetShortLinkAnalyticsHandler getShortLinkAnalyticsHandler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateShortLinkRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateShortLinkCommand(request.OriginalUrl, request.ExpiresAtUtc);

        var result = await createShortLinkHandler.HandleAsync(command, cancellationToken);
        var shortUrl = $"{Request.Scheme}://{Request.Host}/{result.Code}";

        return Created(shortUrl, new CreateShortLinkResponse(
            result.Id,
            result.Code,
            shortUrl,
            result.OriginalUrl,
            result.CreatedAtUtc,
            result.ExpiresAtUtc));
    }

    [HttpGet("{code:length(4,32)}")]
    public async Task<IActionResult> GetAnalytics(string code, CancellationToken cancellationToken)
    {
        var result = await getShortLinkAnalyticsHandler.HandleAsync(code, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(new GetShortLinkAnalyticsResponse(
            result.Id,
            result.Code,
            result.OriginalUrl,
            result.CreatedAtUtc,
            result.ExpiresAtUtc,
            result.ClickCount,
            result.IsExpired));
    }

    public sealed class CreateShortLinkRequest
    {
        [Required]
        [Url]
        [MaxLength(2048)]
        public required string OriginalUrl { get; init; }

        public DateTimeOffset? ExpiresAtUtc { get; init; }
    }

    public sealed record CreateShortLinkResponse(
        Guid Id,
        string Code,
        string ShortUrl,
        string OriginalUrl,
        DateTimeOffset CreatedAtUtc,
        DateTimeOffset? ExpiresAtUtc);

    public sealed record GetShortLinkAnalyticsResponse(
        Guid Id,
        string Code,
        string OriginalUrl,
        DateTimeOffset CreatedAtUtc,
        DateTimeOffset? ExpiresAtUtc,
        long ClickCount,
        bool IsExpired);
}
