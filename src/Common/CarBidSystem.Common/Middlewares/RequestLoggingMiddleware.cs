using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;

namespace CarBidSystem.Common.Middlewares
{
    public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<RequestLoggingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation($"Handling request: {context.Request.Method} {context.Request.Path}");

            await _next(context);

            stopwatch.Stop();
            
            _logger.LogInformation($"Finished handling request. Elapsed time: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
