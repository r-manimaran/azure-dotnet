using EcommerceApp.Host.DTOs.ProductCart;

namespace EcommerceApp.Host.Services;

public interface IProductCartService
{
    Task<ProductCartResponse> GetByIdAsync(Guid id);

    Task<ProductCartResponse> AddAsync(ProductCartRequest request);

    Task<ProductCartResponse> UpdateAsync(Guid cartId, ProductCartRequest request);

    Task DeleteByIdAsync(Guid id);
}
