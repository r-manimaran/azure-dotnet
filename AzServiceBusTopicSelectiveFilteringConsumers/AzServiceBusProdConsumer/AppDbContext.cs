using AzServiceBusProdConsumer.Models;
using Microsoft.EntityFrameworkCore;

namespace AzServiceBusProdConsumer;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }
    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new EventConfiguration());
    }
}
