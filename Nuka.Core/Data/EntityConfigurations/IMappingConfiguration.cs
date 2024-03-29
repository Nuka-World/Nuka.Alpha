﻿using Microsoft.EntityFrameworkCore;

namespace Nuka.Core.Data.EntityConfigurations
{
    /// <summary>
    /// Represents database context model mapping configuration
    /// </summary>
    public interface IMappingConfiguration
    {
        /// <summary>
        /// Apply this mapping configuration
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for the database context</param>
        void ApplyConfiguration(ModelBuilder modelBuilder);
    }
}