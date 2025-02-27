using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace EventTicketingManagementSystem.API.Middlewares
{
    [ExcludeFromCodeCoverage]
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;
        private const int ThresholdInSeconds = 2;
        private readonly List<string> ExcludedPaths = new List<string> { "/notificationHub" };

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
                HttpRequest request = context.Request;
                if (!ExcludedPaths.Contains(request.Path))
                    _logger.LogError("Request {Method} {Path} took too long: {ElapsedSeconds} seconds",
                        request.Method, request.Path, stopwatch.Elapsed.TotalSeconds);
            }
        }
    }
}
