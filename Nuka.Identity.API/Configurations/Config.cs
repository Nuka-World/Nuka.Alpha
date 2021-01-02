using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace Nuka.Identity.API.Configurations
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                // Scope for identity resources. the user claims will in id_token
                new IdentityResource("company", "Company Information",
                    new List<string>() {"department", "location", ClaimTypes.Role})
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("secret_api.access", "Access Secret API")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client()
                {
                    ClientId = "apiClient",
                    ClientName = "ApiClient for Implicit",
                    //显示允许确认页面
                    RequireConsent = true,
                    //客户端授权类型，Implicit:隐藏模式
                    AllowedGrantTypes = GrantTypes.Implicit,
                    //允许登录后重定向的地址列表，可以有多个
                    RedirectUris =
                    {
                        "https://localhost:5002/auth.html",
                        "https://localhost:5002/home/GetTokenData",
                        "https://localhost:5002/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:5002/signout-callback-oidc"
                    },
                    //允许访问的资源
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "secret_api.access",
                        "company"
                    },
                    //允许将token通过浏览器传递
                    AllowAccessTokensViaBrowser = true,
                    //允许ID_TOKEN附带Claims
                    AlwaysIncludeUserClaimsInIdToken = true,
                },
                new Client()
                {
                    //客户端Id
                    ClientId = "apiClientHybrid",
                    ClientName = "ApiClient for Hybrid",
                    //显示允许确认页面
                    RequireConsent = true,
                    //客户端密码
                    ClientSecrets = {new Secret("apiSecret".Sha256())},
                    //客户端授权类型，Hybrid:混合模式
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    //允许登录后重定向的地址列表，可以有多个
                    RedirectUris =
                    {
                        "https://localhost:5002/auth.html",
                        "https://localhost:5002/home/GetTokenData",
                        "https://localhost:5002/signin-oidc"
                    },
                    //注销登录的回调地址列表，可以有多个
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:5002/signout-callback-oidc"
                    },
                    //允许访问的资源
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "secret_api.access",
                        "company"
                    },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 60,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequirePkce = false
                }
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                // secret_api:标识名称
                // Secret Api：显示名称，可以自定义
                // Scopes: 资源拥有的访问作用域
                new ApiResource
                {
                    Name = "secret_api",
                    DisplayName = "Secret Api",
                    Scopes = {"secret_api.access"},
                    UserClaims =
                    {
                        ClaimTypes.Role,
                        "department",
                        "location"
                    }
                }
            };

        public static List<TestUser> Users =>
            new List<TestUser>
            {
                new TestUser()
                {
                    Username = "admin",
                    Password = "admin",
                    SubjectId = "4A2EC065-0A07-4D17-BA71-A9AA040959F4",
                    Claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Role, "admin"), // admin role
                        new Claim("department", "Administrator"), // contain customer claim
                        new Claim("location", "Shanghai")
                    }
                },
                new TestUser()
                {
                    Username = "apiUserGuest",
                    Password = "apiUserPassword",
                    SubjectId = "E293B8CE-9C62-4533-A8A9-7B63DEA53534",
                    Claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Role, "guest"), // guest role
                        new Claim("department", "Developer"), // contain customer claim
                        new Claim("location", "Beijing")
                    }
                }
            };
    }
}