using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Nuka.Identity.API.Data;

namespace Nuka.Identity.API.Factories
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var migrationAssembly = this.GetType().Assembly.GetName().Name;

            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                sqlServerOptionsAction: o => o.MigrationsAssembly(migrationAssembly));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}