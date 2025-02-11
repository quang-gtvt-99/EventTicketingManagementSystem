using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EventTicketingManagementSystem.Middlewares
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;
        private const int ThresholdInSeconds = 2; // Set x to 2 seconds

        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            if (stopwatch.Elapsed.TotalSeconds > ThresholdInSeconds)
            {
                var request = context.Request;
                _logger.LogError("Request {Method} {Path} took too long: {ElapsedSeconds} seconds",
                    request.Method, request.Path, stopwatch.Elapsed.TotalSeconds);
            }
        }
    }
}
