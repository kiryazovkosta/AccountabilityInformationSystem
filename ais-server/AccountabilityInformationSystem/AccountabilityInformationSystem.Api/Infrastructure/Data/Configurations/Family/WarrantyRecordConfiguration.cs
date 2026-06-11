using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Shared.Constants;
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

        builder.Property(e => e.ReceiptId).HasMaxLength(EntitiesConstants.IdMaxLength).IsRequired(false);

        builder.Property(e => e.FrontImageId).HasMaxLength(EntitiesConstants.IdMaxLength).IsRequired(false);

        builder.Property(e => e.BackImageId).HasMaxLength(EntitiesConstants.IdMaxLength).IsRequired(false);

        builder.HasOne(e => e.Receipt)
            .WithMany()
            .HasForeignKey(e => e.ReceiptId);

        builder.HasOne(e => e.FrontImage)
            .WithMany()
            .HasForeignKey(e => e.FrontImageId);

        builder.HasOne(e => e.BackImage)
            .WithMany()
            .HasForeignKey(e => e.BackImageId);
    }
}
