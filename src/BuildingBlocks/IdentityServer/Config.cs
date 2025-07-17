using IdentityModel;
using IdentityServer8.Models;
using IdentityServer8;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource
            {
                Name = "role",
                UserClaims = new List<string> { "role" }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("microservice.api", "Microservice API"),
            new ApiScope("user.api", "User Management API"),
            new ApiScope("order.api", "Order Management API"),
            new ApiScope("payment.api", "Payment API"),
            new ApiScope("notification.api", "Notification API"),
            new ApiScope("full_access", "Full API Access")
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new ApiResource("microservice", "Microservice APIs")
            {
                Scopes = { "microservice.api", "user.api", "order.api", "payment.api", "notification.api", "full_access" },
                UserClaims = new List<string> { JwtClaimTypes.Name, JwtClaimTypes.Email, JwtClaimTypes.Role }
            }
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            // Machine to machine client for microservices
            new Client
            {
                ClientId = "microservice.client",
                ClientName = "Microservice Client",
                ClientSecrets = { new Secret("microservice_secret_2024".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "microservice.api", "user.api", "order.api", "payment.api", "notification.api" }
            },

            // Interactive client for web applications
            new Client
            {
                ClientId = "web.client",
                ClientName = "Web Application",
                ClientSecrets = { new Secret("web_secret_2024".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                
                // Where to redirect to after login
                RedirectUris = { 
                    "https://localhost:5001/signin-oidc",
                    "https://localhost:7001/signin-oidc"
                },
                
                // Where to redirect to after logout
                PostLogoutRedirectUris = { 
                    "https://localhost:5001/signout-callback-oidc",
                    "https://localhost:7001/signout-callback-oidc"
                },
                
                AllowOfflineAccess = true,
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "role",
                    "microservice.api",
                    "user.api"
                }
            },

            // Mobile/SPA client
            new Client
            {
                ClientId = "mobile.client",
                ClientName = "Mobile Application",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RequirePkce = true,
                
                RedirectUris = { 
                    "com.microservice.mobile://callback",
                    "http://localhost:3000/callback"
                },
                
                PostLogoutRedirectUris = { 
                    "com.microservice.mobile://logout",
                    "http://localhost:3000/logout"
                },
                
                AllowedCorsOrigins = { "http://localhost:3000" },
                
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "role",
                    "user.api"
                }
            },

            // Administrative client with full access
            new Client
            {
                ClientId = "admin.client",
                ClientName = "Admin Client",
                ClientSecrets = { new Secret("admin_secret_2024".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                
                RedirectUris = { "https://localhost:6001/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:6001/signout-callback-oidc" },
                
                AllowOfflineAccess = true,
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "role",
                    "full_access"
                }
            }
        };
}
