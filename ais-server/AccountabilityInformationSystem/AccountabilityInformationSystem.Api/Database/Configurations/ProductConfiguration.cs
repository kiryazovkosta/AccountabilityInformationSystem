using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountabilityInformationSystem.Api.Database.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(pt => pt.Id);

        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.NameMaxLength);

        builder.Property(e => e.FullName)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.FullNameMaxLength);

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.ProductConstant.CodeLength);

        builder.Property(e => e.ProductTypeId)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(e => e.ApCodeId)
            .IsRequired(false)
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(e => e.BrandNameId)
            .IsRequired(false)
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(e => e.CnCodeId)
            .IsRequired(false)
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.HasIndex(e => e.Code).IsUnique();
    }
}
