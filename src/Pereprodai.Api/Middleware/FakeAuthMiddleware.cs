using System.Security.Claims;

namespace Pereprodai.Api.Middleware;

public class FakeAuthMiddleware
{
    private readonly RequestDelegate _next;

    public FakeAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue("X-User-Id", out var userId);
        if (Guid.TryParse(userId, out var guid))
        {
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, guid.ToString())
            }));
        }

        await _next(context);
    }
}
