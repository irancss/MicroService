using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

#if DEBUG
public class SimpleDebugController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public SimpleDebugController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<IActionResult> TestUser()
    {
        var user = await _userManager.FindByNameAsync("09124607630");

        if (user == null)
            return Json(new { Error = "User not found" });

        var passwordCheck = await _userManager.CheckPasswordAsync(user, _configuration["AdminUser:DefaultPassword"]);

        return Json(new
        {
            UserId = user.Id,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            IsMobileVerified = user.IsMobileVerified,
            LockoutEnd = user.LockoutEnd,
            AccessFailedCount = user.AccessFailedCount,
            LockoutEnabled = user.LockoutEnabled,
            PasswordValid = passwordCheck,
            PasswordHash = user.PasswordHash?.Substring(0, 20) + "..."
        });
    }

    public async Task<IActionResult> TestLogin()
    {
        var user = await _userManager.FindByNameAsync("09124607630");

        if (user == null)
            return Json(new { Error = "User not found" });

        var result = await _signInManager.PasswordSignInAsync(user, _configuration["AdminUser:DefaultPassword"], false, false);

        return Json(new
        {
            Succeeded = result.Succeeded,
            IsLockedOut = result.IsLockedOut,
            IsNotAllowed = result.IsNotAllowed,
            RequiresTwoFactor = result.RequiresTwoFactor
        });
    }
}
#endif