using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountabilityInformationSystem.Api.Database.Configurations.Identity;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.EmailMaxLength);

        builder.Property(user => user.FirstName)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.FirstNameMaxLength);

        builder.Property(user => user.MiddleName)
            .HasMaxLength(EntitiesConstants.MiddleNameMaxLength);

        builder.Property(user => user.LastName)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.LastNameMaxLength);

        builder.Property(user => user.Image)
            .HasMaxLength(EntitiesConstants.ImageMaxLength);

        builder.Property(user => user.IdentityId)
            .IsRequired()
            .HasMaxLength(EntitiesConstants.IdMaxLength);

        builder.Property(user => user.CreatedBy)
            .HasMaxLength(EntitiesConstants.CreatedByMaxLength);

        builder.Property(user => user.ModifiedBy)
            .HasMaxLength(EntitiesConstants.ModifiedByMaxLength)
            .IsRequired(false);

        builder.HasIndex(user => user.Email).IsUnique();
        builder.HasIndex(user => user.IdentityId).IsUnique();
    }
}
