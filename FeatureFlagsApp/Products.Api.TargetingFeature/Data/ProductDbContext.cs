using Microsoft.EntityFrameworkCore;
using Products.Api.TargetingFeature.Models;

namespace Products.Api.TargetingFeature.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options):base(options)
    {
        
    }
    public DbSet<Product> Products { get; set; }
}
