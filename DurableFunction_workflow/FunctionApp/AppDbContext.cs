using FunctionApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FunctionApp;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<RequestEntity> Requests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<RequestEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RequestData).HasColumnType("nvarchar(max)");
            entity.HasIndex(e=>e.InstanceId);
            entity.HasIndex(e=>e.EmployeeId);
        });
    }
}
