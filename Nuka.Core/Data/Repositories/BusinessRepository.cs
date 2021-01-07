using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Nuka.Core.Data.DBContext;
using Nuka.Core.Data.Entities;

namespace Nuka.Core.Data.Repositories
{
    public class BusinessRepository<TEntity> : IRepository<TEntity> where TEntity : BusinessEntity
    {
        #region Fields

        private readonly IDbContext _context;

        private DbSet<TEntity> _entities;

        #endregion Fields

        #region Ctor

        public BusinessRepository(IDbContext context)
        {
            this._context = context;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public TEntity GetById(object id)
        {
            return Entities.Find(id);
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entities.Add(entity);
            _context.SaveChanges();
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public void Insert(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Entities.AddRange(entities);
            _context.SaveChanges();
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entities.Update(entity);
            _context.SaveChanges();
        }

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public void Update(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Entities.UpdateRange(entities);
            _context.SaveChanges();
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entities.Remove(entity);
            _context.SaveChanges();
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public void Delete(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Entities.RemoveRange(entities);
            _context.SaveChanges();
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public IQueryable<TEntity> Table => Entities;

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

        /// <summary>
        /// Gets an entity set
        /// </summary>
        private DbSet<TEntity> Entities => _entities ??= _context.Set<TEntity>();

        #endregion Properties
    }
}