using AccountabilityInformationSystem.Api.Domain.Entities.Common;
using AccountabilityInformationSystem.Api.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountabilityInformationSystem.Api.Infrastructure.Data.Configurations.Common;

public sealed class StorageFileConfiguration : IEntityTypeConfiguration<StorageFile>
{
    public void Configure(EntityTypeBuilder<StorageFile> builder)
    {
        builder.ToTable("StorageFiles", "ais");

        builder.HasKey(sf => sf.Id);

        builder.Property(sf => sf.Id)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(sf => sf.BlobName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(sf => sf.OriginalFileName)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(sf => sf.ContentType)
            .IsRequired()
            .HasMaxLength(265);
    }
}
