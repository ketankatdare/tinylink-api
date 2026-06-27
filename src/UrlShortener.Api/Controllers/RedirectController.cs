using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.ShortLinks.ResolveForRedirect;

namespace UrlShortener.Api.Controllers;

[ApiController]
public sealed class RedirectController(ResolveShortLinkForRedirectHandler resolveShortLinkForRedirectHandler) : ControllerBase
{
    [HttpGet("{code:length(4,32):regex(^[a-zA-Z0-9]+$)}")]
    public async Task<IActionResult> RedirectToOriginal(string code, CancellationToken cancellationToken)
    {
        var result = await resolveShortLinkForRedirectHandler.HandleAsync(code, cancellationToken);

        if (!result.Found)
        {
            return NotFound();
        }

        if (result.Expired)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }

        return Redirect(result.OriginalUrl!);
    }
}
