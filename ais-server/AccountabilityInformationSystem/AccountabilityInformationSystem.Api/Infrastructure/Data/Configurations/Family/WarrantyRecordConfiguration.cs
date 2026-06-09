using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountabilityInformationSystem.Api.Infrastructure.Data.Configurations.Family;

public sealed class WarrantyRecordConfiguration : IEntityTypeConfiguration<WarrantyRecord>
{
    public void Configure(EntityTypeBuilder<WarrantyRecord> builder)
    {
        builder.ToTable("WarrantyRecords", "family");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(e => e.WarrantyBrandId)
            .HasMaxLength(EntitiesConstants.IdMaxLength)
            .IsRequired();

        builder.Property(e => e.Model).HasMaxLength(EntitiesConstants.WarrantyRecord.ModelMaxLength);

        builder.Property(e => e.Receipt).HasMaxLength(EntitiesConstants.WarrantyRecord.ImageMaxLength).IsRequired(false);

        builder.Property(e => e.FrontImage).HasMaxLength(EntitiesConstants.WarrantyRecord.ImageMaxLength).IsRequired(false);

        builder.Property(e => e.BackImage).HasMaxLength(EntitiesConstants.WarrantyRecord.ImageMaxLength).IsRequired(false);
    }
}
