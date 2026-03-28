using CampusTouch.Application.Common.Exceptions;
using FluentValidation;
namespace CampusTouch.API.Middlewares
{
    public class GlobalExceptionMiddleWare
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            
            catch (FluentValidation.ValidationException ex)
            {
                await HandleException(httpContext, 400, ex.Message);
            }
            catch(CampusTouch.Application.Common.Exceptions.ValidationException ex)
            {
                await HandleException(httpContext, 400, ex.Message);
            }
            catch (UnauthorizedException ex)
            {
                await HandleException(httpContext, 401, ex.Message);
            }
            catch (NotFoundException ex)
            {
                await HandleException(httpContext, 404, ex.Message);
            }
            catch (BuisnessRuleException ex)
            {
                await HandleException(httpContext, 409, ex.Message);
            }
            catch (Exception)
            {
                await HandleException(httpContext, 500, "Internal server error");
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