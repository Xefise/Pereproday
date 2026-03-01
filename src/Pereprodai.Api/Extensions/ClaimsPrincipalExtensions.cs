using System.Security.Claims;

namespace Pereprodai.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        Guid.TryParse(claim?.Value, out var userId);

        if (userId == Guid.Empty) throw new UnauthorizedAccessException();
        return userId;
    }

    public static Guid? GetOptionalUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);

        if (Guid.TryParse(claim?.Value, out var userId)) return userId;
        return null;
    }
}