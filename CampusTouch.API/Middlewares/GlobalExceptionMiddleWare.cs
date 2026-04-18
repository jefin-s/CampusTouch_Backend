using CampusTouch.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CampusTouch.API.Middlewares
{
    public class GlobalExceptionMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleWare> _logger;

        public GlobalExceptionMiddleWare(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleWare> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }

            catch (FluentValidation.ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error");
                await HandleException(httpContext, 400, ex.Message);
            }
            catch (CampusTouch.Application.Common.Exceptions.ValidationException ex)
            {
                _logger.LogWarning(ex, "Custom validation error");
                await HandleException(httpContext, 400, ex.Message);
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access");
                await HandleException(httpContext, 401, ex.Message);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found");
                await HandleException(httpContext, 404, ex.Message);
            }
            catch (BuisnessRuleException ex)
            {
                _logger.LogWarning(ex, "Business rule violation");
                await HandleException(httpContext, 409, ex.Message);
            }
            catch (Exception ex)
            {
                var realMessage = ex.InnerException?.Message ?? ex.Message;

                _logger.LogError(ex, "Unhandled exception occurred");

                await HandleException(httpContext, 500, realMessage);
            }
        }

        private static async Task HandleException(
            HttpContext context,
            int statusCode,
            string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                statusCode = statusCode,
                message = message
            });
        }
    }
}