using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AccountabilityInformationSystem.Api.Infrastructure.Data;

public sealed class ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
    : IdentityDbContext(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(SchemasConstants.Identity);

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .IsRequired()
                .HasMaxLength(EntitiesConstants.IdMaxLength);

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(EntitiesConstants.TokenMaxlength);

            entity.HasIndex(e => e.Token).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder
            .UseSeeding((context, _) =>
            {

            })
            .UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                try
                {
                    RoleManager<IdentityRole> roleManager = context.GetService<RoleManager<IdentityRole>>();
                    if (!await roleManager.RoleExistsAsync(Role.Admin))
                    {
                        await roleManager.CreateAsync(new IdentityRole(Role.Admin));
                    }

                    if (!await roleManager.RoleExistsAsync(Role.FlowUser))
                    {
                        await roleManager.CreateAsync(new IdentityRole(Role.FlowUser));
                    }

                    if (!await roleManager.RoleExistsAsync(Role.Member))
                    {
                        await roleManager.CreateAsync(new IdentityRole(Role.Member));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }


            });
    }
}
