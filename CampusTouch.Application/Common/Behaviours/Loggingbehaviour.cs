using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CampusTouch.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(
            ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            // ✅ Log request start
            _logger.LogInformation(
                "Handling {RequestName} with data {@Request}",
                requestName,
                request);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await next();

                stopwatch.Stop();

                // ✅ Log request success
                _logger.LogInformation(
                    "Handled {RequestName} in {ElapsedMilliseconds} ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                // ❌ Log failure
                _logger.LogError(
                    ex,
                    "Error handling {RequestName} after {ElapsedMilliseconds} ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds);

                throw; // important → let middleware handle response
            }
        }
    }
}