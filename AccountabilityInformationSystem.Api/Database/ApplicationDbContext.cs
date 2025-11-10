using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Entities;
using AccountabilityInformationSystem.Api.Entities.Excise;
using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AccountabilityInformationSystem.Api.Database;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : DbContext(options)
{
    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<Product> Products { get; set; }

    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Ikunk> Ikunks { get; set; }
    public DbSet<MeasurementPoint> MeasurementPoints { get; set; }
    public DbSet<MeasurementPointData> MeasurementPointsData { get; set; }

    public DbSet<ApCode> ApCodes { get; set; }
    public DbSet<ApCode> BrandNames { get; set; }
    public DbSet<ApCode> CnCodes { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemasConstants.Application);

        ApplyDecimalDefaultPrecision(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    private static void ApplyDecimalDefaultPrecision(ModelBuilder modelBuilder)
    {
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (IMutableProperty property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                {
                    property.SetPrecision(18);
                    property.SetScale(3);
                }
            }
        }
    }
}


