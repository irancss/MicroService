using System.Security.Claims;
using BuildingBlocks.Core.Contracts; // [جدید] برای ICurrentUserService
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Identity
{
    /// <summary>
    /// [اصلاح شد] این اینترفیس به ICurrentUserService در لایه Core منتقل شد تا وابستگی به Identity کمتر شود.
    /// این کلاس به عنوان یک پیاده‌سازی مشخص برای محیط‌های وب (HTTP-based) باقی می‌ماند.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}