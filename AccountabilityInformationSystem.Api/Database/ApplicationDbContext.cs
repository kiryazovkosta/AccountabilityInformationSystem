using AccountabilityInformationSystem.Api.Common.Constants;
using AccountabilityInformationSystem.Api.Entities;
using AccountabilityInformationSystem.Api.Entities.Flow;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Database;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : DbContext(options)
{
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Ikunk> Ikunks { get; set; }
    public DbSet<MeasurementPoint> MeasurementPoints { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(ShemasConstants.Application);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}


