using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoadSignCapture.Infrastructure.Cache;
using RoadSignCapture.Core.Services;

namespace RoadSignCapture.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            var redisHost = config["Redis:Host"];
            var redisPort = config["Redis:Port"];
            var connectionString = $"{redisHost}:{redisPort},abortConnect=false";
            Console.WriteLine($" Connecting to Redis at {redisHost}:{redisPort}");

            services.AddSingleton<ICacheService>(new RedisCacheService(connectionString));

            return services;
        }
    }
}
