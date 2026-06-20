using NotesApi.Services;

namespace NotesApi.Middleware;

public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public JwtAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IJwtTokenService jwtTokenService)
    {
        string? authorization = context.Request.Headers.Authorization;

        if (!string.IsNullOrWhiteSpace(authorization) &&
            authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            string token = authorization["Bearer ".Length..].Trim();
            var principal = jwtTokenService.ValidateToken(token);

            if (principal != null)
            {
                context.User = principal;
            }
        }

        await _next(context);
    }
}
