using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using User.Models;
using User.Services;
using Microsoft.EntityFrameworkCore;
using Twilio.Rest.Verify.V2.Service;
using Twilio;

namespace User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Models.User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Models.User> _signInManager;
        private readonly IDatabase _redis;
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;
        private readonly ILogger _logger;


        public AuthController(
            UserManager<Models.User> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] string phoneNumber)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (existingUser != null)
                return BadRequest("User already exists.");

            var user = new Models.User
            {
                UserName = phoneNumber,
                PhoneNumber = phoneNumber,
                PhoneNumberConfirmed = false
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Registration failed for {Email}", phoneNumber);
                return BadRequest(result.Errors);
            }

            // ارسال پیامک تایید (Twilio)
            TwilioClient.Init(_config["Twilio:SID"], _config["Twilio:Token"]);
            await VerificationResource.CreateAsync(to: phoneNumber, channel: "sms", pathServiceSid: _config["Twilio:ServiceSID"]);


            // تخصیص نقش پیش‌فرض
            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            await _userManager.AddToRoleAsync(user, "User");

            _logger.LogInformation("User registered: {Email}", phoneNumber);
            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyRequest req)
        {
            TwilioClient.Init(_config["Twilio:SID"], _config["Twilio:Token"]);
            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: req.PhoneNumber,
                code: req.Code,
                pathServiceSid: _config["Twilio:ServiceSID"]);

            if (verificationCheck.Status != "approved") return BadRequest("Invalid code.");

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == req.PhoneNumber);
            if (user == null) return NotFound();

            user.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(user);

            return Ok("Phone verified.");
        }

        public class VerifyRequest
        {
            public string PhoneNumber { get; set; }
            public string Code { get; set; }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                _logger.LogWarning("Login failed for {Email}", model.PhoneNumber);
                return Unauthorized(new { Message = "Invalid credentials" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            _logger.LogInformation("User logged in: {Email}", model.PhoneNumber);
            return Ok(new { Token = token });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.Email,
                user.PhoneNumber,
                user.Name,
                user.Address
            });
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            user.Name = model.Name ?? user.Name;
            user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            _logger.LogInformation("Profile updated for {Email}", user.Email);
            return Ok(new { Message = "Profile updated successfully" });
        }
    }

    public class UpdateProfileModel
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
    }
}