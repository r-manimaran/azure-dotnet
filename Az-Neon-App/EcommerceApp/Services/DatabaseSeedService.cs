using Bogus;
using Microsoft.EntityFrameworkCore;
using ProductsApp.Domain.Products;
using ProductsApp.Domain.Users;
using ProductsApp.Infrastructure.Database;

namespace EcommerceApp.Host.Services;

public static class DatabaseSeedService
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        if(await dbContext.Products.AnyAsync())
        {
            return;
        }
        var users = GenerateUsers(5);
        var products = GenerateProducts(50);

        await dbContext.Users.AddRangeAsync(users);
        await dbContext.Products.AddRangeAsync(products);

        await dbContext.SaveChangesAsync();
    }
    

    public static List<User> GenerateUsers(int count)
    {
        return new Faker<User>()
            .RuleFor(u => u.Username, f => f.Person.UserName)
            .RuleFor(u => u.Email, f => f.Person.Email)
            .Generate(count);
    }
    public static List<Product> GenerateProducts(int count)
    {
        return new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price()))
            .Generate(count);

    }
}
