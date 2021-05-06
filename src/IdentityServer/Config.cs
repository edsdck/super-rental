using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new("api1", "My API")
            };
        
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new()
                {
                    ClientId = "client",
                    AllowedGrantTypes = new List<string> { "password" },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1", "openid", "profile" }
                },
                new()
                {
                    ClientId = "interactive",
                    ClientSecrets = new List<Secret> { new("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = new List<string> { "http://localhost:5554/signin-oidc" },
                    PostLogoutRedirectUris = new List<string> { "http://localhost:5554/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    RequirePkce = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AllowedScopes = new List<string> { "openid", "profile", "api1", "offline_access" }
                },
                new()
                {
                    ClientId = "mvcclient",
                    ClientSecrets = new List<Secret> { new("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = new List<string> { "http://localhost/mvc/signin-oidc" },
                    PostLogoutRedirectUris = new List<string> { "http://localhost/mvc/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    RequirePkce = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AllowedScopes = new List<string> { "openid", "profile", "api1", "offline_access" }
                }
            };
        
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        
        
    }
}