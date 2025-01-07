using Bogus;
using Bogus.DataSets;
using Products.Api.TargetingFeature.Models;
using System.Runtime.CompilerServices;

namespace Products.Api.TargetingFeature.Data;

public class ProductSeeder
{
    private readonly ILogger<ProductSeeder> _logger;
    private readonly ProductDbContext _dbContext;
    Faker _faker;
    public ProductSeeder(ILogger<ProductSeeder> logger, ProductDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
        _faker = new Faker();
    }

    public async Task SeedAsync()
    {
        if (!_dbContext.Products.Any())
        {
            var products = new List<Product>();
            string[] currencies = new[] { "USD", "INR", "CAD" };
            for(int i=0; i<20;i++)
            {
                var isDiscounted = _faker.Random.Bool();
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = _faker.Commerce.ProductName(),
                    DisplayName = _faker.Commerce.ProductAdjective(),
                    Price = Convert.ToDecimal(_faker.Random.Decimal(50, 2000)),
                    Currency = _faker.PickRandom(currencies),
                    Discounted = isDiscounted,
                    DiscountPercentage = isDiscounted ? Convert.ToDecimal(_faker.Random.Double(5.0, 30.0)):0,
                    Quantity = _faker.Random.Number(1, 200)
                };

                products.Add(product);
            }
            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();
        }
    }
}
