using CarBidSystem.Bids.Plugins.EFCoreSqlServer;
using CarBidSystem.Bids.Service;
using CarBidSystem.Bids.UseCases.Consumers;
using CarBidSystem.Common.Configurations;
using CarBidSystem.Common.Extensions;
using CarBidSystem.Common.Middlewares;
using MassTransit;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
// Add services to the container.
builder.Host.ConfigureSerilog();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextFactory<BidDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BidServiceDb"));
});

builder.Services.AddCommands();
builder.Services.AddValidators();
builder.Services.AddRepositories(builder.Environment);

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("BidLimiter", policy =>
    {
        policy.PermitLimit = 100; // Max 100 requests
        policy.Window = TimeSpan.FromMinutes(1); // Per minute
        policy.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        policy.QueueLimit = 50; // Queue up to 50 requests
    });
});

builder.Services.AddMassTransit(x =>
{
    // Add consumers (message handlers)
    x.AddConsumer<AuctionCreatedConsumer>();

    // Configure RabbitMQ as the transport
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqSettings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
        cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
        {
            h.Username(rabbitMqSettings.Username);
            h.Password(rabbitMqSettings.Password);
        });

        cfg.UseMessageRetry(r => r.Exponential(
            retryLimit: 5,           // Retry 3 times
            minInterval: TimeSpan.FromSeconds(1),  // Minimum delay between retries
            maxInterval: TimeSpan.FromSeconds(30), // Maximum delay between retries
            intervalDelta: TimeSpan.FromSeconds(2) // Exponential backoff factor
        ));

        // Configure the endpoint for the consumer
        cfg.ReceiveEndpoint(rabbitMqSettings.QueueName, e =>
        {
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });
    });
});

if (builder.Environment.IsEnvironment("Docker"))
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        var certPath = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path");
        var certPassword = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password");

        options.ListenAnyIP(80); // HTTP
        options.ListenAnyIP(443, listenOptions =>
        {
            if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(certPassword))
            {
                listenOptions.UseHttps(certPath, certPassword); // HTTPS with certificate
            }
        });
    });
}
else if (!builder.Environment.IsEnvironment("IntegrationTest"))
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(10091); // HTTP
        options.ListenAnyIP(9091, listenOptions =>
        {
            listenOptions.UseHttps(); // HTTPS
        });
    });
}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (!builder.Environment.IsEnvironment("IntegrationTest"))
{
    app.EnsureMigrationOfContext<BidDbContext>();
}
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRateLimiter();
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();


namespace CarBidSystem.Bids.Service
{
    public partial class Program
    { 
    }
}
