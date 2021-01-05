using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuka.Core.Data.Entities;

namespace Nuka.Core.Data.EntityConfigurations
{
    public abstract class BaseEntityTypeConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(entity => entity.Id);

            builder.Property(entity => entity.Id)
                .HasColumnName("FID");

            builder.Property(entity => entity.CreateUser)
                .HasColumnName("FCREATEUSER");

            builder.Property(entity => entity.CreateDateTime)
                .HasColumnName("FCREATETIME");

            builder.Property(entity => entity.UpdateUser)
                .HasColumnName("FUPDATEUSER");

            builder.Property(entity => entity.UpdateDateTime)
                .HasColumnName("FUPDATETIME");
            
            builder.Property(entity => entity.UpdateFunction)
                .HasColumnName("FUPDATEFUNC");
        }
    }
}