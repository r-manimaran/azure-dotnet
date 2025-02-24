using Carter;
using EcommerceApp.Host.DTOs.Product;
using ProductsApp.Domain.Products;

namespace EcommerceApp.Host.Endpoints;

public class ProductsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
                         .WithTags("Products")
                         .WithOpenApi();

        // Get
        group.MapGet("{id:int}", async (int id, IProductRepository productRepository) =>
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return Results.NotFound($"Product with Id {id} not found.");
            }

            var response = new ProductResponse(product.Id, product.Name, product.Price);
            return Results.Ok(response);
        });

        // Create
        group.MapPost("add", async (ProductRequest request, IProductRepository productRepository) =>
        {
            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                Description = request.Description
            };
            await productRepository.AddAsync(product);
            return Results.Created($"/api/Products/{product.Id}", product.Id);
        });

        // Update
        group.MapPut("{id:int}", async (int id, ProductRequest request, IProductRepository productRepository) =>
        {
            var existingProduct = await productRepository.GetByIdAsync(id);
            if(existingProduct == null)
            {
                return Results.NotFound($"Product with ID {id} not found.");
            }

            existingProduct.Name = request.Name;
            existingProduct.Price = request.Price;
            existingProduct.Description = request.Description;
            await productRepository.UpdateAsync(existingProduct);
            return Results.NoContent();
        });

        // Delete
        group.MapDelete("{id:int}", async (int id, IProductRepository productRepository) =>
        {
            await productRepository.DeleteAsync(id);
            return Results.NoContent();
        });
    }

    
}
