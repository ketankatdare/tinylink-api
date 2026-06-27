using UrlShortener.Infrastructure;

namespace UrlShortener.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddHealthChecks();
        services.AddInfrastructureServices(configuration);

        return services;
    }
}
