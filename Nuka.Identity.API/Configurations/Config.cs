using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;

namespace Nuka.Identity.API.Configurations
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }
        
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new ApiResource[]
            {
                new ApiResource
                {
                    Name = "sample.api",
                    DisplayName = "Sample Service",
                    Scopes = {"sample.api.access"},
                },
                new ApiResource
                {
                    Name = "sample.aggregator",
                    DisplayName = "Sample Http Aggregator",
                    Scopes = {"sample.aggregator.access"}
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new ApiScope[]
            {
                new ApiScope("sample.api.access", "Sample API Access"),
                new ApiScope("sample.aggregator.access", "Sample Aggregator Access")
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            return new Client[]
            {
                // MVC Client
                new Client()
                {
                    ClientId = "mvc_web",
                    ClientName = "MVC Web Client",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    ClientSecrets = {new Secret("mvc_secret".Sha256())},
                    RedirectUris = {$"{configuration["URLS:MvcWeb"]}/signin-oidc"},
                    PostLogoutRedirectUris = new List<string> {$"{configuration["URLS:MvcWeb"]}/signout-callback-oidc"},
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "sample.aggregator.access",
                        "sample.api.access",
                    },
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true,
                    RequirePkce = false,
                    RequireConsent = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AccessTokenLifetime = 60 * 60 * 2, // 2 hours
                    IdentityTokenLifetime = 60 * 60 * 2 // 2 hours
                }
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser()
                {
                    Username = "admin",
                    Password = "admin",
                    SubjectId = "4A2EC065-0A07-4D17-BA71-A9AA040959F4",
                },
            };
        }
    }
}