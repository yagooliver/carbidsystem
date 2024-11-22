using CarBidSystem.Auctions.CoreBusiness.Entities;
using CarBidSystem.Auctions.Plugins.EFCoreSqlServer;
using CarBidSystem.Auctions.Service.Services;
using CarBidSystem.Auctions.UseCases.Consumers;
using CarBidSystem.Common.Configurations;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace CarBidSystem.Auctions.IntegrationTests.Factory
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Service.Program>, IAsyncLifetime
    {

        public const string MSSQL_PASSWORD = "dev@1234";

        private MsSqlContainer? sqlContainer;
        private RabbitMqContainer? rabbitMqContainer;
        private RedisContainer? redisContainer;
        public string? SqlConnectionString => sqlContainer?.GetConnectionString();
        public string? RabbitMqHost => rabbitMqContainer?.Hostname;
        public string RedisConnectionString => $"{redisContainer.Hostname}:{redisContainer.GetMappedPublicPort(6379)}";

        public async Task InitializeAsync()
        {
            // Start SQL Server container
            if (sqlContainer == null)
            {
                sqlContainer = new MsSqlBuilder()
                    //.WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
                    .WithPassword(MSSQL_PASSWORD)
                    .Build();

                await sqlContainer.StartAsync();
            }
            await SeedDatabaseAsync();
            // Configure RabbitMQ Container
            if (rabbitMqContainer == null)
            {
                rabbitMqContainer = new RabbitMqBuilder()
                  .WithImage("rabbitmq:3.11")
                  .Build();
            }

            await rabbitMqContainer.StartAsync();

            if (redisContainer == null)
            {
                redisContainer = new RedisBuilder()
                    .WithImage("redis:6.2-alpine")
                    .Build();
            }

            await redisContainer.StartAsync();
        }

        private async Task SeedDatabaseAsync()
        {
            // Create a DbContext with the connection string
            var options = new DbContextOptionsBuilder<AuctionDbContext>()
                .UseSqlServer(SqlConnectionString)
                .Options;

            using var context = new AuctionDbContext(options);

            // Ensure the database schema is created
            await context.Database.EnsureCreatedAsync();

            // Seed test data
            if (!await context.Cars.AnyAsync())
            {
                await context.Cars.AddRangeAsync([
                    new("Toyota", "Corolla", 2020, 15000m),
                    new("Honda", "Civic", 2019, 16000m),
                    new("Ford", "Mustang", 2021, 30000m),
                    new("Chevrolet", "Camaro", 2022, 35000m),
                    new("Tesla", "Model 3", 2023, 45000m),
                    new("BMW", "3 Series", 2018, 25000m),
                    new("Mercedes-Benz", "C-Class", 2020, 40000m),
                    new("Audi", "A4", 2021, 38000m),
                    new("Volkswagen", "Passat", 2019, 20000m),
                    new("Hyundai", "Elantra", 2022, 18000m)
                ]);

                await context.SaveChangesAsync();
            }
        }


        public async Task<Auction?> GetAuctionByIdAsync(int auctionId)
        {
            // Create a DbContext with the connection string
            var options = new DbContextOptionsBuilder<AuctionDbContext>()
                .UseSqlServer(SqlConnectionString)
                .Options;

            using var context = new AuctionDbContext(options);

            // Seed test data
            Auction? auction = await context.Auctions.FirstOrDefaultAsync(x => x.Id == auctionId);

            return auction;
        }

        public async Task AddAuctionAsync(Auction auction)
        {
            var options = new DbContextOptionsBuilder<AuctionDbContext>()
                .UseSqlServer(SqlConnectionString)
                .Options;

            using var context = new AuctionDbContext(options);

            context.Auctions.Add(auction);

            await context.SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            if(sqlContainer != null) await sqlContainer.StopAsync();
            if(rabbitMqContainer != null) await rabbitMqContainer.StopAsync();
            if (redisContainer != null) await redisContainer.StopAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Replace SQL Server with containerized instance
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AuctionDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AuctionDbContext>(options =>
                {
                    options.UseSqlServer(SqlConnectionString);
                });

                // Override RabbitMQ configuration
                services.Configure<RabbitMqSettings>(config =>
                {
                    config.Host = RabbitMqHost;
                    config.VirtualHost = "/";
                    config.Username = "guest";
                    config.Password = "guest";
                    config.QueueName = "bid-service-test";
                });

                services.AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<PlaceBidCommandConsumer>();

                    //x.UsingRabbitMq((context, cfg) =>
                    //{
                    //    var rabbitMqSettings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
                    //    cfg.Host(rabbitMqSettings.Host, "/", h =>
                    //    {
                    //        h.Username(rabbitMqSettings.Username);
                    //        h.Password(rabbitMqSettings.Password);
                    //    });

                    //    cfg.ReceiveEndpoint(rabbitMqSettings.QueueName, e =>
                    //    {
                    //        e.ConfigureConsumer<PlaceBidCommandConsumer>(context);
                    //    });
                    //});
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                });

                // Replace Redis configuration
                var redisDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IConnectionMultiplexer));
                if (redisDescriptor != null)
                {
                    services.Remove(redisDescriptor);
                }

                services.AddSingleton<IConnectionMultiplexer>(_ =>
                {
                    return ConnectionMultiplexer.Connect(RedisConnectionString);
                });

                services.AddScoped(sp =>
                {
                    var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
                    return multiplexer.GetDatabase();
                });

                // Remove the hosted services
                var hostedServiceTypes = new[]
                {
                    typeof(StartUpcomingAuctionsService),
                    typeof(EndExpiredAuctionsService)
                };

                foreach (var hostedServiceType in hostedServiceTypes)
                {
                    var hostedServiceDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == hostedServiceType);
                    if (hostedServiceDescriptor != null)
                    {
                        services.Remove(hostedServiceDescriptor);
                    }
                }
            });

            // Set the test environment
            builder.UseEnvironment("IntegrationTest");
        }
    }
}
