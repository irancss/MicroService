using IdentityModel;
using IdentityServer8.Test;
using IdentityServer8;
using System.Security.Claims;
using System.Text.Json;

namespace IdentityServer;

public static class TestUsers
{
    public static List<TestUser> Users =>
        new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "1",
                Username = "admin",
                Password = "admin123",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Admin User"),
                    new Claim(JwtClaimTypes.GivenName, "Admin"),
                    new Claim(JwtClaimTypes.FamilyName, "User"),
                    new Claim(JwtClaimTypes.Email, "admin@microservice.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.Role, "Administrator"),
                    new Claim(JwtClaimTypes.WebSite, "https://admin.microservice.com"),
                    new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(new
                    {
                        street_address = "One Admin Way",
                        locality = "Tehran",
                        postal_code = "12345",
                        country = "Iran"
                    }), IdentityServerConstants.ClaimValueTypes.Json)
                }
            },
            
            new TestUser
            {
                SubjectId = "2",
                Username = "user",
                Password = "user123",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Regular User"),
                    new Claim(JwtClaimTypes.GivenName, "Regular"),
                    new Claim(JwtClaimTypes.FamilyName, "User"),
                    new Claim(JwtClaimTypes.Email, "user@microservice.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.Role, "User"),
                    new Claim(JwtClaimTypes.WebSite, "https://user.microservice.com"),
                    new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(new
                    {
                        street_address = "One User Street",
                        locality = "Tehran",
                        postal_code = "54321",
                        country = "Iran"
                    }), IdentityServerConstants.ClaimValueTypes.Json)
                }
            },

            new TestUser
            {
                SubjectId = "3",
                Username = "manager",
                Password = "manager123",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Manager User"),
                    new Claim(JwtClaimTypes.GivenName, "Manager"),
                    new Claim(JwtClaimTypes.FamilyName, "User"),
                    new Claim(JwtClaimTypes.Email, "manager@microservice.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.Role, "Manager"),
                    new Claim(JwtClaimTypes.WebSite, "https://manager.microservice.com"),
                    new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(new
                    {
                        street_address = "One Manager Avenue",
                        locality = "Tehran",
                        postal_code = "67890",
                        country = "Iran"
                    }), IdentityServerConstants.ClaimValueTypes.Json)
                }
            }
        };
}
