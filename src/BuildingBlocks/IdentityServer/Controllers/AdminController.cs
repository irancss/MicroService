using IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Data;

namespace IdentityServer.Controllers;

[Authorize(AuthenticationSchemes = "Identity.Application")]
[Route("admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AdminController> _logger;
    private readonly ApplicationDbContext _context;

    public AdminController(
        UserManager<ApplicationUser> userManager, 
        ILogger<AdminController> logger,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var totalUsers = await _userManager.Users.CountAsync();
        var activeUsers = await _userManager.Users.CountAsync(u => u.IsActive);
        var inactiveUsers = totalUsers - activeUsers;
        var verifiedUsers = await _userManager.Users.CountAsync(u => u.IsMobileVerified);
        var todayRegistrations = await _userManager.Users.CountAsync(u => u.CreatedAt.Date == DateTime.UtcNow.Date);

        var recentUsers = await _userManager.Users
            .OrderByDescending(u => u.CreatedAt)
            .Take(10)
            .Select(u => new RecentUser
            {
                Id = u.Id,
                PhoneNumber = u.PhoneNumber ?? "",
                FirstName = u.FirstName,
                LastName = u.LastName,
                RegisteredAt = u.CreatedAt,
                IsActive = u.IsActive,
                IsMobileVerified = u.IsMobileVerified
            })
            .ToListAsync();

        // Simulate some recent logs (in production, pull from actual logging system)
        var recentLogs = new List<SystemLog>
        {
            new() { Level = "Info", Message = "User login successful", Timestamp = DateTime.UtcNow.AddMinutes(-5) },
            new() { Level = "Warning", Message = "Failed SMS attempt", Timestamp = DateTime.UtcNow.AddMinutes(-10) },
            new() { Level = "Info", Message = "Database migration completed", Timestamp = DateTime.UtcNow.AddHours(-1) }
        };

        var model = new AdminDashboardViewModel
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            InactiveUsers = inactiveUsers,
            VerifiedUsers = verifiedUsers,
            TodayRegistrations = todayRegistrations,
            RecentUsers = recentUsers,
            RecentLogs = recentLogs
        };

        return View(model);
    }

    [HttpGet("users")]
    public async Task<IActionResult> Users(int page = 1, string search = "", bool? isActive = null)
    {
        const int pageSize = 20;
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.PhoneNumber!.Contains(search) || 
                                   (u.FirstName != null && u.FirstName.Contains(search)) || 
                                   (u.LastName != null && u.LastName.Contains(search)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        var totalUsers = await query.CountAsync();
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserViewModel
            {
                Id = u.Id,
                PhoneNumber = u.PhoneNumber ?? "",
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                LastLoginDate = u.LastLoginDate,
                EmailConfirmed = u.EmailConfirmed,
                PhoneNumberConfirmed = u.PhoneNumberConfirmed,
                IsMobileVerified = u.IsMobileVerified,
                LockoutEndDate = u.LockoutEndDate
            })
            .ToListAsync();

        var model = new UsersListViewModel
        {
            Users = users,
            CurrentPage = page,
            TotalCount = totalUsers,
            SearchTerm = search,
            IsActiveFilter = isActive
        };

        return View(model);
    }

    [HttpPost("users/{id}/toggle-status")]
    public async Task<IActionResult> ToggleUserStatus(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.IsActive = !user.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            _logger.LogInformation("وضعیت کاربر {UserId} تغییر کرد به {Status}", id, user.IsActive ? "فعال" : "غیرفعال");
            return Json(new { success = true, isActive = user.IsActive });
        }

        return Json(new { success = false });
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            _logger.LogInformation("کاربر {UserId} حذف شد", id);
            return Json(new { success = true });
        }

        return Json(new { success = false, errors = result.Errors });
    }

    [HttpGet("settings")]
    public IActionResult Settings()
    {
        return View();
    }

    [HttpGet("logs")]
    public IActionResult Logs()
    {
        return View();
    }

    [HttpGet("tokens")]
    public async Task<IActionResult> Tokens(int page = 1, string search = "", string tokenType = "", bool? isActive = null)
    {
        const int pageSize = 20;
        
        // For now, show active user sessions as a simpler approach
        var query = _userManager.Users.AsQueryable();
        
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.PhoneNumber!.Contains(search) || 
                                   (u.FirstName != null && u.FirstName.Contains(search)) || 
                                   (u.LastName != null && u.LastName.Contains(search)));
        }

        var users = await query
            .Where(u => u.LastLoginDate.HasValue) // Users who have logged in
            .OrderByDescending(u => u.LastLoginDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var tokenViewModels = users.Select(u => new TokenViewModel
        {
            Subject = u.Id,
            ClientId = "mobile-app", // Default client
            CreationTime = u.LastLoginDate ?? DateTime.UtcNow,
            ExpirationTime = u.LastLoginDate?.AddDays(30), // Assume 30-day expiry
            Type = "access_token",
            IsActive = u.IsActive && u.IsMobileVerified
        }).ToList();

        var totalCount = await query.Where(u => u.LastLoginDate.HasValue).CountAsync();

        var model = new TokensListViewModel
        {
            Tokens = tokenViewModels,
            CurrentPage = page,
            TotalCount = totalCount,
            SearchTerm = search,
            TokenType = tokenType,
            IsActive = isActive
        };

        return View(model);
    }

    [HttpGet("tokens/{userId}")]
    public async Task<IActionResult> TokenDetail(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var model = new TokenDetailViewModel
        {
            Subject = user.Id,
            ClientId = "mobile-app",
            CreationTime = user.LastLoginDate ?? DateTime.UtcNow,
            ExpirationTime = user.LastLoginDate?.AddDays(30),
            Type = "access_token",
            IsActive = user.IsActive && user.IsMobileVerified,
            Claims = new Dictionary<string, string>
            {
                ["sub"] = user.Id,
                ["phone_number"] = user.PhoneNumber ?? "",
                ["given_name"] = user.FirstName ?? "",
                ["family_name"] = user.LastName ?? "",
                ["phone_number_verified"] = user.IsMobileVerified.ToString()
            }
        };

        return View(model);
    }

    [HttpPost("tokens/revoke-user/{userId}")]
    public async Task<IActionResult> RevokeUserTokens(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        try
        {
            // In a real implementation, you would revoke actual tokens here
            // For now, we'll just deactivate the user temporarily
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            
            _logger.LogInformation("توکن‌های کاربر {UserId} لغو شد", userId);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در لغو توکن‌های کاربر {UserId}", userId);
            return Json(new { success = false, error = "خطا در لغو توکن‌ها" });
        }
    }
}
