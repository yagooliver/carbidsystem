using CarBidSystem.Bids.CoreBusiness.Interfaces;
using CarBidSystem.Bids.Plugins.EFCoreSqlServer.Repositories;
using CarBidSystem.Bids.UseCases.Behaviors;
using CarBidSystem.Bids.UseCases.Bids.Handlers;
using CarBidSystem.Bids.UseCases.Bids.Validator;
using FluentValidation;
using MediatR;

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
            if (environment.IsEnvironment("Testing"))
            {
                services.AddSingleton<IAuctionRepository, Plugins.InMemory.AuctionRepository>();
                services.AddSingleton<IBidRepository, Plugins.InMemory.BidRepository>();
            }
            else
            {
                services.AddScoped<IAuctionRepository, AuctionRepository>();
                services.AddScoped<IBidRepository, BidRepository>();
            }
        }
    }
}
