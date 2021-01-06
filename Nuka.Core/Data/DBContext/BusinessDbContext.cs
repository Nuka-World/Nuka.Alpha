using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nuka.Core.Data.Entities;

namespace Nuka.Core.Data.DBContext
{
    public abstract class BusinessDbContext : DbContext
    {
        protected BusinessDbContext(DbContextOptions options) : base(options)
        {
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is BusinessEntity entity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            entity.LastUpdatedAt = DateTime.UtcNow;
                            break;

                        case EntityState.Added:
                            entity.CreatedAt = DateTime.UtcNow;
                            entity.LastUpdatedAt = DateTime.UtcNow;
                            entity.CacheIndex = Guid.NewGuid();
                            break;

                        case EntityState.Detached:
                        case EntityState.Unchanged:
                        case EntityState.Deleted:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}