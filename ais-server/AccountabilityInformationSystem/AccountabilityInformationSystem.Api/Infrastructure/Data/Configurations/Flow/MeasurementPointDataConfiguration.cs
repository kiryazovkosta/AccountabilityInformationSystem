using AccountabilityInformationSystem.Api.Shared.Constants;
using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountabilityInformationSystem.Api.Infrastructure.Data.Configurations.Flow;

public sealed class MeasurementPointDataConfiguration : IEntityTypeConfiguration<MeasurementPointData>
{
    public void Configure(EntityTypeBuilder<MeasurementPointData> builder)
    {
        builder.ToTable("MeasurementPointData", "flow");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(e => e.MeasurementPointId)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(e => e.Number)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(e => e.BeginTime)
            .IsRequired();

        builder.Property(e => e.EndTime)
            .IsRequired();

        builder.Property(e => e.FlowDirectionType)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.EnumMaxLength)
            .HasConversion(
                val => val.ToString(),
                val => Enum.Parse<FlowDirectionType>(val));

        builder.Property(e => e.ProductId)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(e => e.TotalizerBeginGrossObservableVolume)
            .IsRequired(false);

        builder.Property(e => e.TotalizerEndGrossObservableVolume)
            .IsRequired(false);

        builder.Property(e => e.TotalizerBeginGrossStandardVolume)
            .IsRequired(false);

        builder.Property(e => e.TotalizerEndGrossStandardVolume)
            .IsRequired(false);

        builder.Property(e => e.TotalizerBeginMass)
            .IsRequired(false);

        builder.Property(e => e.TotalizerEndMass)
            .IsRequired(false);

        builder.Property(e => e.GrossObservableVolume)
            .IsRequired(false);

        builder.Property(e => e.GrossStandardVolume)
            .IsRequired(false);

        builder.Property(e => e.Mass)
            .IsRequired(false);

        builder.Property(e => e.AverageObservableDensity)
            .IsRequired(false);

        builder.Property(e => e.AverageReferenceDensity)
            .IsRequired(false);

        builder.Property(e => e.AverageTemperature)
            .IsRequired(false);

        builder.Property(e => e.AlcoholContent)
            .IsRequired(false);

        builder.Property(e => e.BatchNumber)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(e => e.ExternalId)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(EntitiesConstants.CreatedByMaxLength);

        builder.Property(e => e.ModifiedBy)
            .HasMaxLength(EntitiesConstants.ModifiedByMaxLength)
            .IsRequired(false);

        builder.HasOne(e => e.MeasurementPoint)
            .WithMany()
            .HasForeignKey(e => e.MeasurementPointId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder.HasOne(e => e.Product)
            .WithMany()
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder.HasIndex(e => e.MeasurementPointId);
        builder.HasIndex(e => e.Number);
        builder.HasIndex(e => e.FlowDirectionType);
        builder.HasIndex(e => e.BeginTime);
        builder.HasIndex(e => e.EndTime);
        builder.HasIndex(e => e.ProductId);

        builder.HasIndex(e => new { e.MeasurementPointId, e.FlowDirectionType, e.Number, e.BeginTime, e.EndTime })
            .IsUnique();
    }
}
