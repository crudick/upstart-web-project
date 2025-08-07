using Microsoft.AspNetCore.Antiforgery;

namespace Upstart.Api.Endpoints;

public static class CsrfEndpoint
{
    public static void MapCsrfEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/csrf").WithTags("CSRF");

        group.MapGet("/token", GetCsrfToken)
            .WithName("GetCsrfToken")
            .WithSummary("Get CSRF token for form submissions")
            .Produces<CsrfTokenResponse>(200);
    }

    private static IResult GetCsrfToken(HttpContext httpContext, IAntiforgery antiforgery)
    {
        var tokens = antiforgery.GetAndStoreTokens(httpContext);
        var response = new CsrfTokenResponse(tokens.RequestToken!);
        return Results.Ok(response);
    }
}

public record CsrfTokenResponse(string Token);