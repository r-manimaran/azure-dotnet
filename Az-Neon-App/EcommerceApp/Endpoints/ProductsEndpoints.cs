using Carter;
using EcommerceApp.Host.DTOs;
using ProductsApp.Domain.Products;

namespace EcommerceApp.Host.Endpoints;

public class ProductsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
                         .WithTags("Products")
                         .WithOpenApi();

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


        group.MapPost("add", async () =>
        {
            return Results.Ok();
        });
    }

    
}
