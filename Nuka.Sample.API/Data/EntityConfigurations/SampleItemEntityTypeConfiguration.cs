using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuka.Core.EntityConfigurations;
using Nuka.Sample.API.Data.Entities;

namespace Nuka.Sample.API.Data.EntityConfigurations
{
    public class SampleItemEntityTypeConfiguration : BaseEntityTypeConfiguration<SampleItem>
    {
        public override void Configure(EntityTypeBuilder<SampleItem> builder)
        {
            base.Configure(builder);

            builder.ToTable("ITEMM");

            builder.Property(item => item.Name)
                .HasColumnName("FNAME")
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(item => item.Price)
                .HasColumnName("FPRICE")
                .IsRequired(true);

            builder.Property(item => item.Description)
                .HasColumnName("FDISCIPTION")
                .IsRequired(false);
            
            builder.Property(item => item.SampleTypeId)
                .HasColumnName("FTYPEID");

            builder.HasOne(item => item.SampleType)
                .WithMany()
                .HasForeignKey(item => item.SampleTypeId);
        }
    }
}