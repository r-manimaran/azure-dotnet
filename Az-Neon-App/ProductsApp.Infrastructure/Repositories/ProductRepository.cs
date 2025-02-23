using Microsoft.EntityFrameworkCore;
using ProductsApp.Domain.Products;
using ProductsApp.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsApp.Infrastructure.Repositories;

public class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    public async Task AddAsync(Product product)
    {
        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        dbContext.Products.Remove(new Product { Id = id });
        await dbContext.SaveChangesAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var product = await dbContext.Products.Where(i => i.Id == id).FirstOrDefaultAsync();
        if (product == null)
        {
            throw new Exception("Product Not found");
        }
        return product;
    }

 
}
