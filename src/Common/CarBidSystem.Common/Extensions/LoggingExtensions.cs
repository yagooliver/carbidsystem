using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Elastic.Serilog.Sinks;
using Elastic.Apm.SerilogEnricher;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Ingest.Elasticsearch;


namespace CarBidSystem.Common.Extensions
{
    public static class LoggingExtensions
    {
        public static IHostBuilder ConfigureSerilog(this IHostBuilder builder)
        {
            return builder.UseSerilog((context, services, configuration) =>
            {
                try
                {
                    if (!context.HostingEnvironment.IsEnvironment("IntegrationTest"))
                    {
                        // Read settings from appsettings.json
                        string? elasticUri = context.Configuration["ElasticConfiguration:Uri"];
                        configuration.ReadFrom.Configuration(context.Configuration)
                                     .WriteTo.Console(Serilog.Events.LogEventLevel.Verbose) // Always log to console
                                     .WriteTo.Elasticsearch(new[] { new Uri(elasticUri ?? string.Empty) }, opts =>
                                     {
                                         opts.DataStream = new DataStreamName("logs",
                                             $"carbid-system-{context.Configuration["ElasticConfiguration:ServiceName"]}",
                                             context.HostingEnvironment.EnvironmentName);

                                         opts.BootstrapMethod = BootstrapMethod.Failure;

                                         opts.ConfigureChannel = channelOpts =>
                                         {
                                             channelOpts.BufferOptions = new BufferOptions
                                             {
                                                 ExportMaxConcurrency = 10
                                             };
                                         };
                                     });
                    }
                }
                catch (Exception ex)
                {
                    // Log the error and fallback to console logging
                    Log.Information($"Failed to configure Elasticsearch logging: {ex.Message}");

                    configuration.WriteTo.Console(Serilog.Events.LogEventLevel.Warning);
                }
            });
        }
    }
}
