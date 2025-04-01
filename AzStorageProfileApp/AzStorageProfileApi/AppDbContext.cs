using AzStorageProfileApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AzStorageProfileApi;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }
    public DbSet<UserProfile> userProfiles { get; set; }
}
