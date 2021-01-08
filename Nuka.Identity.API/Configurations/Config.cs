using System.Collections.Generic;
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
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[] { };

        public static IEnumerable<Client> Clients =>
            new Client[] { };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[] { };

        public static List<TestUser> Users =>
            new List<TestUser>();
    }
}