using Microsoft.EntityFrameworkCore;
using ProductsApp.Domain.Products;
using ProductsApp.Domain.Users;

namespace ProductsApp.Infrastructure.Database;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCart> ProductCarts { get; set; }
    public DbSet<ProductCartItem> ProductCartItems { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(DatabaseConst.Schema);

        modelBuilder.UseIdentityByDefaultColumns();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
