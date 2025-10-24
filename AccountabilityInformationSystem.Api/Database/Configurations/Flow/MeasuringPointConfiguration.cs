using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Entities.Flow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountabilityInformationSystem.Api.Database.Configurations.Flow;

public sealed class MeasuringPointConfiguration : IEntityTypeConfiguration<MeasurementPoint>
{
    public void Configure(EntityTypeBuilder<MeasurementPoint> builder)
    {
        builder.ToTable("MeasurementPoints", "flow");

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

        builder.Property(e => e.ControlPoint)
            .HasMaxLength(EntitiesConstants.ControlPointMaxLength);

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(EntitiesConstants.CreatedByMaxLength);

        builder.Property(e => e.ModifiedBy)
            .HasMaxLength(EntitiesConstants.ModifiedByMaxLength)
            .IsRequired(false);

        builder.Property(e => e.IkunkId)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(e => e.FlowDirection)
            .HasMaxLength(EntitiesConstants.EnumMaxLength)
            .HasConversion(
                val => val.ToString(),
                val => Enum.Parse<FlowDirectionType>(val));

        builder.Property(e => e.Transport)
            .HasMaxLength(EntitiesConstants.EnumMaxLength)
            .HasConversion(
                val => val.ToString(),
                val => Enum.Parse<TransportType>(val));

        builder.HasIndex(e => e.Name)
            .IsUnique();

        builder.HasIndex(e => e.ControlPoint)
            .IsUnique();
    }
}
