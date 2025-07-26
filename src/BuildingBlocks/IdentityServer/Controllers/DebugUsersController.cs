using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Controllers;

#if DEBUG
public class DebugUsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DebugUsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> ListUsers()
    {
        var users = await _userManager.Users.Take(10).ToListAsync();
        var result = users.Select(u => new
        {
            u.Id,
            u.UserName,
            u.PhoneNumber,
            u.Email,
            u.IsActive,
            u.IsMobileVerified,
            u.EmailConfirmed,
            u.PhoneNumberConfirmed,
            u.LockoutEnabled,
            u.LockoutEnd,
            HasPassword = !string.IsNullOrEmpty(u.PasswordHash)
        }).ToList();

        return Json(result);
    }

    [HttpGet]
    public async Task<IActionResult> CheckUser(string phoneNumber = "09124607630")
    {
        var user = await _userManager.FindByNameAsync(phoneNumber);
        if (user == null)
        {
            return Json(new { found = false, message = "کاربر یافت نشد" });
        }

        var result = new
        {
            found = true,
            user = new
            {
                user.Id,
                user.UserName,
                user.PhoneNumber,
                user.Email,
                user.IsActive,
                user.IsMobileVerified,
                user.EmailConfirmed,
                user.PhoneNumberConfirmed,
                user.LockoutEnabled,
                user.LockoutEnd,
                user.AccessFailedCount,
                HasPassword = !string.IsNullOrEmpty(user.PasswordHash),
                PasswordHash = user.PasswordHash?.Substring(0, 20) + "...",
                user.SecurityStamp
            }
        };

        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> TestPassword(string phoneNumber = "09124607630", string password = "Ji'%w@4o03c|Gc.qKK")
    {
        var user = await _userManager.FindByNameAsync(phoneNumber);
        if (user == null)
        {
            return Json(new { success = false, message = "کاربر یافت نشد" });
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, password);

        return Json(new
        {
            success = passwordValid,
            message = passwordValid ? "رمز عبور صحیح است" : "رمز عبور اشتباه است",
            user = new
            {
                user.Id,
                user.UserName,
                user.IsActive,
                user.LockoutEnabled,
                user.LockoutEnd,
                user.AccessFailedCount
            }
        });
    }
}

#endif