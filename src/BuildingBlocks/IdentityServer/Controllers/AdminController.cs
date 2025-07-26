using IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Data;
using IdentityServer.TempModels;
using IdentityServer8.Stores;
using TempModels;

namespace IdentityServer.Controllers;

[Authorize(AuthenticationSchemes = "Identity.Application")]
[Route("admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AdminController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IPersistedGrantStore _persistedGrantStore;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        ILogger<AdminController> logger,
        ApplicationDbContext context,
        RoleManager<IdentityRole> roleManager,
        IPersistedGrantStore persistedGrantStore)
    {
        _userManager = userManager;
        _logger = logger;
        _context = context;
        _roleManager = roleManager;
        _persistedGrantStore = persistedGrantStore;
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

    [HttpGet("roles")]
    public async Task<IActionResult> Roles()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return View(roles);
    }
    [HttpPost("roles/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
    {
        if (ModelState.IsValid)
        {
            var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!roleExists)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.RoleName));
                if (result.Succeeded)
                {
                    _logger.LogInformation("Role جدید ایجاد شد: {RoleName}", model.RoleName);
                    return RedirectToAction("Roles");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "این Role از قبل وجود دارد.");
            }
        }
        // اگر به اینجا برسد یعنی خطایی وجود دارد
        var roles = await _roleManager.Roles.ToListAsync();
        return View("Roles", roles); // برگرداندن به همان صفحه با خطاها
    }

    [HttpGet("users/{userId}/manage-roles")]
    public async Task<IActionResult> ManageUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var model = new ManageUserRolesViewModel
        {
            UserId = user.Id,
            UserName = user.UserName
        };

        var rolesViewModel = new List<UserRoleViewModel>();
        foreach (var role in await _roleManager.Roles.ToListAsync())
        {
            rolesViewModel.Add(new UserRoleViewModel
            {
                RoleName = role.Name,
                IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
            });
        }

        model.Roles = rolesViewModel;
        return View(model);
    }


    [HttpPost("users/{userId}/manage-roles")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return NotFound();
        }

        // گرفتن Roleهای فعلی کاربر
        var userRoles = await _userManager.GetRolesAsync(user);

        // حذف تمام Roleهای فعلی
        var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
        if (!removeResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "خطا در حذف Roleهای قبلی.");
            return View(model);
        }

        // افزودن Roleهای جدید انتخاب شده
        var addResult = await _userManager.AddToRolesAsync(user, model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName));
        if (!addResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "خطا در افزودن Roleهای جدید.");
            return View(model);
        }

        _logger.LogInformation("Roleهای کاربر {UserId} بروزرسانی شد.", user.Id);
        return RedirectToAction("Users"); // یا به صفحه جزئیات کاربر
    }


    [HttpGet("users/{userId}/manage-claims")]
    public async Task<IActionResult> ManageUserClaims(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var model = new ManageUserClaimsViewModel
        {
            UserId = userId,
            UserName = user.UserName,
            ExistingClaims = (await _userManager.GetClaimsAsync(user)).ToList()
        };

        return View(model);
    }

    [HttpPost("users/{userId}/add-claim")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddUserClaim(ManageUserClaimsViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null) return NotFound();

        if (!string.IsNullOrEmpty(model.ClaimType) && !string.IsNullOrEmpty(model.ClaimValue))
        {
            var newClaim = new Claim(model.ClaimType, model.ClaimValue);
            var result = await _userManager.AddClaimAsync(user, newClaim);

            if (result.Succeeded)
            {
                _logger.LogInformation("Claim جدید برای کاربر {UserId} اضافه شد: {ClaimType}={ClaimValue}", user.Id, newClaim.Type, newClaim.Value);
                return RedirectToAction("ManageUserClaims", new { userId = model.UserId });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        // اگر خطایی بود، دوباره صفحه را با اطلاعات قبلی لود کن
        model.ExistingClaims = (await _userManager.GetClaimsAsync(user)).ToList();
        return View("ManageUserClaims", model);
    }


    [HttpPost("users/{userId}/remove-claim")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveUserClaim(string userId, string claimType, string claimValue)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var claimToRemove = new Claim(claimType, claimValue);
        var result = await _userManager.RemoveClaimAsync(user, claimToRemove);

        if (result.Succeeded)
        {
            _logger.LogInformation("Claim از کاربر {UserId} حذف شد: {ClaimType}={ClaimValue}", user.Id, claimType, claimValue);
        }

        return RedirectToAction("ManageUserClaims", new { userId });
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
   public async Task<IActionResult> Tokens(int page = 1, string search = "", string tokenType = "refresh_token", bool? isActive = true)
    {
        const int pageSize = 20;
        
        
        // 1. ساختن فیلتر برای خواندن گرنت‌ها
    // ما معمولا به دنبال Refresh Tokenها هستیم، چون آن‌ها نماینده یک Session فعال هستند.
    var grantFilter = new PersistedGrantFilter
    {
        Type = string.IsNullOrEmpty(tokenType) ? null : tokenType
    };

    // 2. خواندن تمام گرنت‌های فعال از دیتابیس
    // متد GetAllAsync صفحه‌بندی (Pagination) ندارد، پس همه را می‌خوانیم و در حافظه صفحه‌بندی می‌کنیم.
    // برای سیستم‌های با حجم بسیار بالا، باید راه‌حل بهینه‌تری پیاده کرد.
    var allGrants = (await _persistedGrantStore.GetAllAsync(grantFilter)).AsQueryable();

 // 3. اعمال فیلتر جستجو (Search)
    if (!string.IsNullOrEmpty(search))
    {
        // جستجو بر اساس SubjectId یا ClientId
        allGrants = allGrants.Where(g => (g.SubjectId != null && g.SubjectId.Contains(search)) || 
                                          g.ClientId.Contains(search));
    }

      // 4. شمارش کل نتایج قبل از صفحه‌بندی
    var totalCount = allGrants.Count();

    // 5. اعمال صفحه‌بندی
    var pagedGrants = allGrants
        .OrderByDescending(g => g.CreationTime)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

      // 6. تبدیل گرنت‌ها به ViewModel برای نمایش در UI
    var tokenViewModels = pagedGrants.Select(grant => new TokenViewModel
    {
        Subject = grant.SubjectId,
        ClientId = grant.ClientId,
        SessionId = grant.SessionId,
        CreationTime = grant.CreationTime,
        ExpirationTime = grant.Expiration,
        Type = grant.Type,
        IsActive = !grant.Expiration.HasValue || grant.Expiration.Value > DateTime.UtcNow,
        // Scopes را از JSON داخل Data استخراج می‌کنیم
        Scopes = System.Text.Json.JsonDocument.Parse(grant.Data)
                                                .RootElement.GetProperty("Scopes")
                                                .EnumerateArray()
                                                .Select(e => e.GetString() ?? "")
                                                .ToList()
    }).ToList();

     var model = new TokensListViewModel
    {
        Tokens = tokenViewModels,
        CurrentPage = page,
        TotalCount = totalCount,
        SearchTerm = search,
        TokenType = tokenType,
        IsActive = isActive // فیلتر IsActive در اینجا بیشتر جنبه نمایشی دارد
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

              // 2. این بخش کلیدی برای لغو توکن‌ها در IdentityServer است (بسیار عالی)
            await _persistedGrantStore.RemoveAllAsync(new PersistedGrantFilter { SubjectId = userId });
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
