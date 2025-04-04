using AzFileProcessing.Common;
using Microsoft.EntityFrameworkCore;

namespace FileIngestionProcessor;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }
    public DbSet<Event> Events { get; set; }
}
