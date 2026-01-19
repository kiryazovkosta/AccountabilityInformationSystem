using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountabilityInformationSystem.Api.Infrastructure.Data.Configurations;

public sealed class ProductTypeConfiguration : IEntityTypeConfiguration<ProductType>
{
    public void Configure(EntityTypeBuilder<ProductType> builder)
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

        builder.HasMany(e => e.Products)
            .WithOne(e => e.ProductType)
            .HasForeignKey(e => e.ProductTypeId)
            .IsRequired();
    }
}
