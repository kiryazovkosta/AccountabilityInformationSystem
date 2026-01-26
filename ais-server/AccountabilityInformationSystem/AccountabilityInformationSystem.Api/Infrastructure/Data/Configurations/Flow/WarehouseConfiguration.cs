using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountabilityInformationSystem.Api.Infrastructure.Data.Configurations.Flow;

public sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(e => e.Name)
            .HasMaxLength(EntitiesConstants.NameMaxLength);

        builder.Property(e => e.FullName)
            .HasMaxLength(EntitiesConstants.FullNameMaxLength);

        builder.Property(e => e.Description)
            .HasMaxLength(EntitiesConstants.DescriptionMaxLength)
            .IsRequired(false);

        builder.Property(e => e.ExciseNumber)
            .HasMaxLength(EntitiesConstants.ExciseNumberLength);

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(EntitiesConstants.CreatedByMaxLength);

        builder.Property(e => e.ModifiedBy)
            .HasMaxLength(EntitiesConstants.ModifiedByMaxLength)
            .IsRequired(false);

        builder.HasIndex(e => e.ExciseNumber)
            .IsUnique();

        builder.HasMany(e => e.Ikunks)
            .WithOne(e => e.Warehouse)
            .HasForeignKey(e => e.WarehouseId)
            .IsRequired();
    }
}
