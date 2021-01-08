using Microsoft.EntityFrameworkCore;

namespace Nuka.Core.Data.DBContext
{
    public abstract class BaseDbContext: DbContext, IDbContext
    {
        protected BaseDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}