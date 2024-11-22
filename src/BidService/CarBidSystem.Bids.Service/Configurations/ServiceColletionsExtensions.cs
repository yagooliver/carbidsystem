using CarBidSystem.Bids.CoreBusiness.Interfaces;
using CarBidSystem.Bids.Plugins.EFCoreSqlServer.Repositories;
using CarBidSystem.Bids.Plugins.RedisCaching;
using CarBidSystem.Bids.UseCases.Behaviors;
using CarBidSystem.Bids.UseCases.Bids.Handlers;
using CarBidSystem.Bids.UseCases.Bids.Validator;
using FluentValidation;
using MediatR;
using StackExchange.Redis;

namespace CarBidSystem.Bids.Service
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
            services.AddValidatorsFromAssembly(typeof(PlaceBidCommandValidator).Assembly);
        }

        public static void AddRepositories(this IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddScoped<AuctionRepository>();
            services.AddScoped<IAuctionRepository>(provider =>
            {
                var repository = provider.GetRequiredService<AuctionRepository>();
                var redis = provider.GetRequiredService<IDatabase>();
                return new AuctionCacheRepository(repository, redis);
            });
            services.AddScoped<IBidRepository, BidRepository>();
        }
    }
}
