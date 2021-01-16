using System.IO;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Nuka.Identity.API.Factories
{
    public class PersistedGrantDbContextFactory : IDesignTimeDbContextFactory<PersistedGrantDbContext>
    {
        public PersistedGrantDbContext CreateDbContext(string[] args)
        {
            var migrationAssembly = this.GetType().Assembly.GetName().Name;

            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
            var storeOptions = new OperationalStoreOptions();

            optionsBuilder.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                sqlServerOptionsAction: o => o.MigrationsAssembly(migrationAssembly));

            return new PersistedGrantDbContext(optionsBuilder.Options, storeOptions);
        }
    }
}