using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Entities.Flow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountabilityInformationSystem.Api.Database.Configurations.Flow;

public sealed class IkunkConfiguration : IEntityTypeConfiguration<Ikunk>
{
    public void Configure(EntityTypeBuilder<Ikunk> builder)
    {
        builder.ToTable("Ikunks", "flow");

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

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(EntitiesConstants.CreatedByMaxLength);

        builder.Property(e => e.ModifiedBy)
            .HasMaxLength(EntitiesConstants.ModifiedByMaxLength)
            .IsRequired(false);

        builder.Property(e => e.WarehouseId)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.HasIndex(e => e.Name)
            .IsUnique();

        builder.HasMany(e => e.MeasurementPoints)
            .WithOne(e => e.Ikunk)
            .HasForeignKey(e => e.IkunkId)
            .IsRequired();
    }
}
