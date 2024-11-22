using CarBidSystem.Auctions.Plugins.EFCoreSqlServer.Repositories;
using CarBidSystem.Auctions.Plugins.RedisCaching;
using CarBidSystem.Auctions.UseCases.Auctions.Handlers;
using CarBidSystem.Auctions.UseCases.Auctions.Validators;
using CarBidSystem.Auctions.UseCases.Behaviors;
using CarBidSystem.Auctions.CoreBusiness.Interfaces;
using FluentValidation;
using MediatR;
using StackExchange.Redis;

namespace CarBidSystem.Auctions.Service
{
    public static class ServiceCollectionsExtensions
    {
        public static void AddCommands(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateAuctionCommandHandler).Assembly));
        }

        public static void AddValidators(this IServiceCollection services) 
        {            
            services.AddValidatorsFromAssembly(typeof(CreateAuctionCommandValidator).Assembly);
        }

        public static void AddRepositories(this IServiceCollection services, IWebHostEnvironment environment)
        {
            if(environment.IsEnvironment("Testing"))
            {
                services.AddSingleton<IAuctionRepository, Plugins.InMemory.AuctionRepository>();
                services.AddSingleton<ICarRepository, Plugins.InMemory.CarRepository>();
            }
            else
            {
                services.AddScoped<AuctionRepository>();
                services.AddScoped<IAuctionRepository>(provider =>
                {
                    var repository = provider.GetRequiredService<AuctionRepository>();
                    var redis = provider.GetRequiredService<IDatabase>();
                    return new AuctionCacheRepository(repository, redis);
                });
                services.AddScoped<ICarRepository, CarRepository>();
            }
        }

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
