using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountabilityInformationSystem.Api.Infrastructure.Data.Configurations.Family;

public sealed class WarrantyBrandConfiguration : IEntityTypeConfiguration<WarrantyBrand>
{
    public void Configure(EntityTypeBuilder<WarrantyBrand> builder)
    {
        builder.ToTable("WarrantyBrands", "family");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(e => e.Name).HasMaxLength(EntitiesConstants.NameMaxLength);

        builder.Property(e => e.Logo)
            .HasMaxLength(EntitiesConstants.WarrantyBrand.LogoMaxLength)
            .IsRequired(false);

        builder.HasMany(e => e.WarrantyRecords)
            .WithOne(e => e.WarrantyBrand)
            .HasForeignKey(e => e.WarrantyBrandId)
            .IsRequired();
    }
}
