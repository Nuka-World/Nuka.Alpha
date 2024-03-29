﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuka.Core.Data.Entities;

namespace Nuka.Core.Data.EntityConfigurations
{
    /// <summary>
    /// Represents base entity mapping configuration
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract class MappingEntityTypeConfiguration<TEntity> : IMappingConfiguration,
        IEntityTypeConfiguration<TEntity> where TEntity : BusinessEntity
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public abstract void Configure(EntityTypeBuilder<TEntity> builder);

        /// <summary>
        /// Apply this mapping configuration
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for the database context</param>
        public virtual void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(this);
        }

        #endregion
    }
}