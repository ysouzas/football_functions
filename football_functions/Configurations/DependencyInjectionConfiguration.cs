using System;
using football_functions.Options;
using football_functions.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace football_functions.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static IServiceCollection AddDependencyInjectionConfiguration(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IPlayerTableStorage, PlayerTableStorage>();
            return services;
        }
    }
}

