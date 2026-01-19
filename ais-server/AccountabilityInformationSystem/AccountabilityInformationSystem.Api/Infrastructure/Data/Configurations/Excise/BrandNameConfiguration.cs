using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountabilityInformationSystem.Api.Infrastructure.Data.Configurations.Excise;

public sealed class BrandNameConfiguration : IEntityTypeConfiguration<BrandName>
{
    public void Configure(EntityTypeBuilder<BrandName> builder)
    {
        builder.ToTable("BrandNames", "excise");

        builder.HasKey(bn => bn.Id);

        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.BrandNameConstants.CodeLength);

        builder.Property(e => e.BgDescription)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.ExciseDescriptionMaxLength);

        builder.Property(e => e.DescriptionEn)
            .IsRequired(false)
            .HasMaxLength(EntitiesConstants.ExciseDescriptionMaxLength);

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(EntitiesConstants.CreatedByMaxLength);

        builder.Property(e => e.ModifiedBy)
            .HasMaxLength(EntitiesConstants.ModifiedByMaxLength)
            .IsRequired(false);

        builder.HasMany(e => e.Products)
            .WithOne(e => e.BrandName)
            .HasForeignKey(e => e.BrandNameId)
            .IsRequired(false);
    }
}
