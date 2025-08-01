using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using TestProject.Core.Interfaces;
using TestProject.Infrastructure.Repositories;
using TestProject.Application.Services;
using TestProject.Application.Interfaces;
using TestProject.Infrastructure.Services;

namespace TestProject.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["DefaultConnection"];
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString, x => x.UseNetTopologySuite()));
            services.AddScoped<IMeteoriteRepository, MeteoriteRepository>();
            services.AddScoped<IMeteoriteSyncService, MeteoriteSyncService>();
            services.AddHttpClient<IMeteoritesExternalDataService, MeteoritesExternalDataService>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddMemoryCache();
            return services;
        }
    }
}
