using Carter;
using EcommerceApp.Host.DTOs.ProductCart;
using EcommerceApp.Host.Services;

namespace EcommerceApp.Host.Endpoints;

public class ProductCartsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var route = app.MapGroup("/api/products-carts/").WithOpenApi().WithTags("products-carts");

        route.MapGet("{cartId:guid}", async (Guid cartId, IProductCartService productCartService) =>
        {
            var productCartResponse = await productCartService.GetByIdAsync(cartId);
            if (productCartResponse is null)
            {
                return Results.NotFound($"ProductCart with Id {cartId} not found.");
            }
            return Results.Ok(productCartResponse);
        });

        route.MapPost("", async (ProductCartRequest request, IProductCartService productCartService) =>
        {
            var response = await productCartService.AddAsync(request);
            return Results.Created($"/product-carts/{response.Id}", response);
        });

        route.MapPut("{cartId:guid}", async (Guid cartId, ProductCartRequest request, IProductCartService productCartService) =>
        {
            var existingProductCart = await productCartService.GetByIdAsync(cartId);
            if (existingProductCart is null)
            {
                return Results.NotFound($"ProductCart with ID {cartId} not found.");
            }

            if (existingProductCart.UserId != request.UserId)
            {
                return Results.BadRequest("Cannot update a ProductCart with a different UserId.");
            }

            await productCartService.UpdateAsync(cartId, request);

            return Results.NoContent();
        });

        route.MapDelete("{cartId:guid}", async (Guid cartId, IProductCartService productCartService) =>
        {
            await productCartService.DeleteByIdAsync(cartId);
            return Results.NoContent();
        });
    }
}
