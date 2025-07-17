using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

public class SimpleDebugController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public SimpleDebugController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> TestUser()
    {
        var user = await _userManager.FindByNameAsync("09123456789");
        
        if (user == null)
            return Json(new { Error = "User not found" });
            
        var passwordCheck = await _userManager.CheckPasswordAsync(user, "Admin123!");
        
        return Json(new {
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
        var user = await _userManager.FindByNameAsync("09123456789");
        
        if (user == null)
            return Json(new { Error = "User not found" });
            
        var result = await _signInManager.PasswordSignInAsync(user, "Admin123!", false, false);
        
        return Json(new {
            Succeeded = result.Succeeded,
            IsLockedOut = result.IsLockedOut,
            IsNotAllowed = result.IsNotAllowed,
            RequiresTwoFactor = result.RequiresTwoFactor
        });
    }
}
