using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuka.Core.Data.EntityConfigurations;
using Nuka.Sample.API.Data.Entities;

namespace Nuka.Sample.API.Data.EntityConfigurations
{
    public class SampleTypeEntityTypeConfiguration : BusinessEntityTypeConfiguration<SampleType>
    {
        public override void Configure(EntityTypeBuilder<SampleType> builder)
        {
            base.Configure(builder);

            builder.ToTable("TYPEM");
            
            builder.Property(type => type.Type)
                .HasColumnName("FTYPE")
                .IsRequired(true)
                .HasMaxLength(50);
        }
    }
}