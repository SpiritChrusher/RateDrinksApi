using RateDrinksApi.Data;
using RateDrinksApi.Options;

namespace RateDrinksApi.Extensions;

public static class ServiceConfigurationExtensions
{
    public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration configuration, string appName)
    {
        services.AddOpenApi();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy => policy
                    .AllowAnyOrigin() // Update as needed
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            );
        });

        // Database config (bind from app-scoped section)
        services.Configure<DatabaseOptions>(configuration.GetSection($"{appName}:DatabaseSettings"));
        services.Configure<CosmosDbOptions>(configuration.GetSection($"{appName}:DatabaseSettings:CosmosDb"));

        services.AddHttpClient("HtmlTemplateClient");
        services.AddSingleton<CosmosDrinksDb>();

        return services;
    }
}
