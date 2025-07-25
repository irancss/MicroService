﻿using IdentityServer8.Models;

namespace AuthService
{
    public static class IdentityServerConfig
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("api1", "My API")
            };


        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>()
            {
                new Client()
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes = { "api1" }
                }
            };
    }
}
