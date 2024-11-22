using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CarBidSystem.Common.Extensions
{
    public static class RedisConfiguration
    {
        public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                string? config = configuration.GetConnectionString("RedisConnection");
                return config is null
                    ? throw new ArgumentNullException("Redis Connection string cannot be null")
                    : (IConnectionMultiplexer)ConnectionMultiplexer.Connect(config);
            });

            services.AddScoped(sp =>
            {
                var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
                return multiplexer.GetDatabase();
            });
        }
    }
}
