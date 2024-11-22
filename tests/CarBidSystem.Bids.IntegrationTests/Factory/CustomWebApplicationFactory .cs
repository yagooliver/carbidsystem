using CarBidSystem.Bids.CoreBusiness.Entities;
using CarBidSystem.Bids.Plugins.EFCoreSqlServer;
using CarBidSystem.Bids.Service;
using CarBidSystem.Bids.UseCases.Consumers;
using CarBidSystem.Common.Configurations;
using FluentAssertions.Common;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace CarBidSystem.Bids.IntegrationTests.Configurations
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
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
            var options = new DbContextOptionsBuilder<BidDbContext>()
                .UseSqlServer(SqlConnectionString)
                .Options;

            using var context = new BidDbContext(options);

            // Ensure the database schema is created
            await context.Database.EnsureCreatedAsync();

            // Seed test data
            if (!await context.Auctions.AnyAsync())
            {
                context.Auctions.Add(new(1, DateTime.UtcNow, DateTime.UtcNow.AddDays(2)));

                await context.SaveChangesAsync();
            }
        }

        public async Task AddEndedAuction()
        {
            // Create a DbContext with the connection string
            var options = new DbContextOptionsBuilder<BidDbContext>()
                .UseSqlServer(SqlConnectionString)
                .Options;

            using var context = new BidDbContext(options);

            // Seed test data
            context.Auctions.Add(new(2, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow));

            await context.SaveChangesAsync();
        }

        public async Task AddBidAsync(Bid bid)
        {
            // Create a DbContext with the connection string
            var options = new DbContextOptionsBuilder<BidDbContext>()
                .UseSqlServer(SqlConnectionString)
                .Options;

            using var context = new BidDbContext(options);

            // Seed test data
            context.Bids.Add(bid);

            await context.SaveChangesAsync();
        }

        public async Task<Auction?> GetAuctionByIdAsync(int auctionId)
        {
            // Create a DbContext with the connection string
            var options = new DbContextOptionsBuilder<BidDbContext>()
                .UseSqlServer(SqlConnectionString)
                .Options;

            using var context = new BidDbContext(options);

            // Seed test data
            Auction? auction = await context.Auctions.FirstOrDefaultAsync(x => x.AuctionId == auctionId);

            return auction;
        }

        public async Task DisposeAsync()
        {
            await sqlContainer.StopAsync();
            await rabbitMqContainer.StopAsync();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Replace SQL Server with containerized instance
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<BidDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<BidDbContext>(options =>
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
                //var massTransitDescriptors = services
                //    .Where(d => d.ServiceType.Namespace == "MassTransit" || d.ImplementationType?.Namespace == "MassTransit")
                //    .ToList();

                //foreach (var desc in massTransitDescriptors)
                //{
                //    services.Remove(desc);
                //}
                //services.AddMassTransitTestHarness(x =>
                //{
                //    x.AddConsumer<AuctionCreatedConsumer>();

                //    x.UsingRabbitMq((context, cfg) =>
                //    {
                //        var rabbitMqSettings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
                //        cfg.Host(rabbitMqSettings.Host, "/", h =>
                //        {
                //            h.Username(rabbitMqSettings.Username);
                //            h.Password(rabbitMqSettings.Password);
                //        });

                //        cfg.ReceiveEndpoint(rabbitMqSettings.QueueName, e =>
                //        {
                //            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
                //        });
                //    });
                //});

                services.AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<AuctionCreatedConsumer>();
                    x.AddConsumer<AuctionStartedConsumer>();
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
            });

            // Set the test environment
            builder.UseEnvironment("IntegrationTest");
        }
    }
}
