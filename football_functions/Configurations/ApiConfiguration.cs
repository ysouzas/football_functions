using football_functions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace football_functions.Configurations;

public static class ApiConfiguration
{
    public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ConnectionStrings>(config.GetSection(nameof(ConnectionStrings)));
        return services;
    }
}

