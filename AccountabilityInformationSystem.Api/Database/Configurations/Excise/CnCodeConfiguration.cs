using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Entities.Excise;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountabilityInformationSystem.Api.Database.Configurations.Excise;

public sealed class CnCodeConfiguration : IEntityTypeConfiguration<CnCode>
{
    public void Configure(EntityTypeBuilder<CnCode> builder)
    {
        builder.ToTable("CnCodes", "excise");

        builder.HasKey(cn => cn.Id);

        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.CnCodeConstant.CodeLength);

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
            .WithOne(e => e.CnCode)
            .HasForeignKey(e => e.CnCodeId)
            .IsRequired(false);
    }
}
