using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.ShortLinks.CreateShortLink;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("api/short-links")]
public sealed class ShortLinksController(CreateShortLinkHandler createShortLinkHandler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateShortLinkRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateShortLinkCommand(request.OriginalUrl, request.ExpiresAtUtc);

        try
        {
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
        catch (ArgumentException exception)
        {
            return ValidationProblem(
                title: "Invalid short link request.",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
        catch (InvalidOperationException exception)
        {
            return Problem(
                title: "Unable to generate a unique short code.",
                detail: exception.Message,
                statusCode: StatusCodes.Status409Conflict);
        }
    }

    public sealed record CreateShortLinkRequest(
        [property: Required]
        [property: Url]
        [property: MaxLength(2048)]
        string OriginalUrl,
        DateTimeOffset? ExpiresAtUtc);

    public sealed record CreateShortLinkResponse(
        Guid Id,
        string Code,
        string ShortUrl,
        string OriginalUrl,
        DateTimeOffset CreatedAtUtc,
        DateTimeOffset? ExpiresAtUtc);
}
