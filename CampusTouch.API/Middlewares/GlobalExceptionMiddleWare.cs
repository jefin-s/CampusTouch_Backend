using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CampusTouch.API.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class GlobalExceptionMiddleWare
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class GlobalExceptionMiddleWareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleWare(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleWare>();
        }
    }
}
