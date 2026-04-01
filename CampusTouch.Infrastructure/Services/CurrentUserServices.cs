using CampusTouch.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class CurrentUserServices : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContext;

    public CurrentUserServices(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }

    public string UserId =>
        _httpContext.HttpContext?.User?
        .FindFirst(ClaimTypes.NameIdentifier)?.Value;
    public bool IsAdmin =>
        _httpContext.HttpContext?.User?.IsInRole("Admin") ?? false;
}