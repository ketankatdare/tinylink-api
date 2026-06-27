using UrlShortener.Infrastructure;
using UrlShortener.Api.Middleware;

namespace UrlShortener.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddProblemDetails();
        services.AddHealthChecks();
        services.AddInfrastructureServices(configuration);

        return services;
    }

    public static IApplicationBuilder UseApiMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }
}
