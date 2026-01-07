using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Entities.Excise;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountabilityInformationSystem.Api.Database.Configurations.Excise;

public sealed class ApCodeConfiguration : IEntityTypeConfiguration<ApCode>
{
    public void Configure(EntityTypeBuilder<ApCode> builder)
    {
        builder.ToTable("ApCodes", "excise");

        builder.HasKey(ap => ap.Id);

        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.ApCodeConstants.CodeLength);

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

        builder.HasIndex(e => e.Code)
            .IsUnique();

        builder.HasMany(e => e.Products)
            .WithOne(e => e.ApCode)
            .HasForeignKey(e => e.ApCodeId)
            .IsRequired(false);
    }
}
