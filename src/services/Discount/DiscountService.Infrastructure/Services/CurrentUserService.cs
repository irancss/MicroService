using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using DiscountService.Application.Interfaces;

namespace DiscountService.Infrastructure.Services;

/// <summary>
/// Service to get information about the current user
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public string? UserEmail =>
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsAdmin =>
        _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;
}
