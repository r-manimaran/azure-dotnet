using EcommerceApp.Host.DTOs.ProductCart;
using ProductsApp.Domain.Products;

namespace EcommerceApp.Host.Services;

public class ProductCartService(IProductCartRepository repository, 
                                IProductRepository productRepository) : IProductCartService
{
    public async Task<ProductCartResponse> AddAsync(ProductCartRequest request)
    {
        var productCart = new ProductCart
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            CartItems = request.ProductCartItems.Select(x => x.MapToCartItem()).ToList()
        };
        await repository.AddAsync(productCart);

        productCart = await repository.GetByIdAsync(productCart.Id);
        return productCart!.MapToProductCartResponse();
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        await repository.DeleteAsync(id);
    }

    public async Task<ProductCartResponse?> GetByIdAsync(Guid id)
    {
        var productCart = await repository.GetByIdAsync(id);
        return productCart?.MapToProductCartResponse();
    }

    public async Task<ProductCartResponse> UpdateAsync(Guid cartId, ProductCartRequest request)
    {
        var productCart = await repository.GetByIdAsync(cartId);
        if (productCart is null)
        {
            return null;
        }

        productCart.CartItems = request.ProductCartItems.Select(x => x.MapToCartItem()).ToList();

        await repository.UpdateAsync(productCart);

        var productCartResponse = productCart.MapToProductCartResponse();
        return productCartResponse;
    }
}

