using football_functions.Services;
using football_functions.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace football_functions.Configurations;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddDependencyInjectionConfiguration(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IDealer, Dealer>();
        services.AddScoped<IPlayerTableStorage, PlayerTableStorage>();
        services.AddScoped<IConfigTableStorage, ConfigTableStorage>();
        return services;
    }
}

