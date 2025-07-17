using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Identity
{
    /// <summary>
    /// Interface for accessing user context information
    /// </summary>
    public interface IUserContext
    {
        string? UserId { get; }
        string? UserName { get; }
        string[]? Roles { get; }
        Dictionary<string, string> Claims { get; }
        bool IsAuthenticated { get; }
    }

    /// <summary>
    /// Default implementation of user context
    /// </summary>
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
        public string[]? Roles => _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToArray();
        public Dictionary<string, string> Claims => _httpContextAccessor.HttpContext?.User?.Claims?.ToDictionary(c => c.Type, c => c.Value) ?? new Dictionary<string, string>();
        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
