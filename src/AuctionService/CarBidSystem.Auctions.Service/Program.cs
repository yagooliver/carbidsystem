using CarBidSystem.Auctions.Plugins.EFCoreSqlServer;
using CarBidSystem.Auctions.Service;
using CarBidSystem.Auctions.Service.Services;
using CarBidSystem.Auctions.UseCases.Consumers;
using CarBidSystem.Common.Configurations;
using CarBidSystem.Common.Extensions;
using CarBidSystem.Common.Middlewares;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.ConfigureSerilog();

builder.Services.AddControllers();

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<BackgroundServiceConfig>(
    builder.Configuration.GetSection("BackgroundService"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextFactory<AuctionDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuctionServiceDb"));
});

builder.Services.AddCommands();
builder.Services.AddValidators();
builder.Services.AddRepositories(builder.Environment);
builder.Services.ConfigureRedis(builder.Configuration);
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PlaceBidCommandConsumer>();    
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqSettings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
        cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
        {
            h.Username(rabbitMqSettings.Username);
            h.Password(rabbitMqSettings.Password);
        });

        cfg.UseMessageRetry(r => r.Exponential(
            retryLimit: 5,
            minInterval: TimeSpan.FromSeconds(1),
            maxInterval: TimeSpan.FromSeconds(30),
            intervalDelta: TimeSpan.FromSeconds(2)
        ));
        
        cfg.ReceiveEndpoint(rabbitMqSettings.QueueName, e =>
        {
            e.ConfigureConsumer<PlaceBidCommandConsumer>(context);
        });
    });
});

builder.Services.AddHostedService<StartUpcomingAuctionsService>();
builder.Services.AddHostedService<EndExpiredAuctionsService>();

if(builder.Environment.IsEnvironment("Docker"))
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
else if(!builder.Environment.IsEnvironment("IntegrationTest"))
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(10090); // HTTP
        options.ListenAnyIP(9090, listenOptions =>
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
    app.EnsureMigrationOfContext<AuctionDbContext>();
}
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();


namespace CarBidSystem.Auctions.Service
{
    public partial class Program { }
}