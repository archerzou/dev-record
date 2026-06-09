using DevRecord.Api.Middleware;

namespace DevRecord.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseETag(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ETagMiddleware>();
    }

    public static IApplicationBuilder UseUserContextEnrichment(this IApplicationBuilder app)
    {
        return app.UseMiddleware<UserContextEnrichmentMiddleware>();
    }
}
