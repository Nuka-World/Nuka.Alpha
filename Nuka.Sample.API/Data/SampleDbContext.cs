using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Nuka.Sample.API.Data.Entities;
using Nuka.Sample.API.Data.EntityConfigurations;

namespace Nuka.Sample.API.Data
{
    public class SampleDbContext : DbContext
    {
        public SampleDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<SampleItem> SampleItem { get; set; }
        public DbSet<SampleType> SampleType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SampleItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SampleTypeEntityTypeConfiguration());
        }
    }

    public class SampleDbContextFactory : IDesignTimeDbContextFactory<SampleDbContext>
    {
        public SampleDbContext CreateDbContext(string[] args)
        {
            var migrationAssembly = this.GetType().Assembly.GetName().Name;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SampleDbContext>()
                .UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptionsAction: o => o.MigrationsAssembly(migrationAssembly));

            return new SampleDbContext(optionsBuilder.Options);
        }
    }
}