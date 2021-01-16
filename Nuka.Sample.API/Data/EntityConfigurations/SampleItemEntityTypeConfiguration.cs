using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuka.Core.Data.EntityConfigurations;
using Nuka.Sample.API.Data.Entities;

namespace Nuka.Sample.API.Data.EntityConfigurations
{
    public class SampleItemEntityTypeConfiguration : BusinessEntityTypeConfiguration<SampleItem>
    {
        public override void Configure(EntityTypeBuilder<SampleItem> builder)
        {
            base.Configure(builder);

            builder.ToTable("ITEMM");

            builder.Property(item => item.ItemId)
                .HasColumnName("FITEMID")
                .HasMaxLength(10);

            builder.Property(item => item.ItemName)
                .HasColumnName("FITEMNAME")
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(item => item.Price)
                .HasColumnName("FPRICE")
                .HasPrecision(18, 2)
                .IsRequired(true);

            builder.Property(item => item.Description)
                .HasColumnName("FDISCIPTION");

            builder.Property(item => item.SampleTypeId)
                .HasColumnName("FTYPEID");

            builder.HasOne(item => item.SampleType)
                .WithMany()
                .HasForeignKey(item => item.SampleTypeId);
        }
    }
}