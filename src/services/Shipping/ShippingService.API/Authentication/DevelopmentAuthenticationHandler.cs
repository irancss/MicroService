using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ShippingService.API.Authentication
{
    /// <summary>
    /// Development authentication handler که در محیط Development همه درخواست‌ها را تأیید می‌کند
    /// در Production حذف کنید و از Auth microservice استفاده کنید
    /// </summary>
    public class DevelopmentAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public DevelopmentAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, UrlEncoder encoder) 
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // در محیط Development همه کاربران را Admin فرض می‌کنیم
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "DevelopmentUser"),
                new Claim(ClaimTypes.NameIdentifier, "dev-user-id"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("scope", "shipping.read"),
                new Claim("scope", "shipping.write"),
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
