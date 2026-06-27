using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.ShortLinks.CreateShortLink;
using UrlShortener.Application.ShortLinks.GetAnalytics;
using UrlShortener.Application.ShortLinks.ResolveForRedirect;
using UrlShortener.Infrastructure.Persistence;
using UrlShortener.Infrastructure.Persistence.Repositories;
using UrlShortener.Infrastructure.Services;

namespace UrlShortener.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is not configured.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<CreateShortLinkHandler>();
        services.AddScoped<GetShortLinkAnalyticsHandler>();
        services.AddScoped<ResolveShortLinkForRedirectHandler>();
        services.AddScoped<IShortLinkRepository, ShortLinkRepository>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IShortCodeGenerator, RandomShortCodeGenerator>();

        return services;
    }
}
