using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.OrderAggregate;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply Configuration
        modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
    }
}