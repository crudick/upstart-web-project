using System.Security.Claims;

namespace Upstart.Api.Extensions;

public static class HttpContextExtensions
{
    public static int GetUserId(this HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return userId;
    }

    public static string GetUserEmail(this HttpContext httpContext)
    {
        var emailClaim = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(emailClaim))
        {
            throw new UnauthorizedAccessException("User email not found in token");
        }
        return emailClaim;
    }
}