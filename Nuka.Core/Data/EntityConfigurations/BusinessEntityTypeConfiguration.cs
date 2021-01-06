using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuka.Core.Data.Entities;

namespace Nuka.Core.Data.EntityConfigurations
{
    public abstract class BusinessEntityTypeConfiguration<TEntity> : MappingEntityTypeConfiguration<TEntity>
        where TEntity : BusinessEntity
    {
        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(entity => entity.Id);

            builder.Property(entity => entity.Id)
                .HasColumnName("FID")
                .ValueGeneratedOnAdd();

            builder.Property(entity => entity.CreatedBy)
                .HasColumnName("FCREATEDUSER");

            builder.Property(entity => entity.CreatedAt)
                .HasColumnName("FCREATEDTIME");

            builder.Property(entity => entity.LastUpdatedBy)
                .HasColumnName("FUPDATEDUSER");

            builder.Property(entity => entity.LastUpdatedAt)
                .HasColumnName("FUPDATEDTIME");

            builder.Property(entity => entity.LastUpdatedFunc)
                .HasColumnName("FUPDATEDFUNCTION");

            builder.Property(entity => entity.CacheIndex)
                .HasColumnName("FCACHEINDEX");
        }
    }
}