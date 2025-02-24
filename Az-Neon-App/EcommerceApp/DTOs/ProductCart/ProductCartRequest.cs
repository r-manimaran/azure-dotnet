using ProductsApp.Domain.Products;

namespace EcommerceApp.Host.DTOs.ProductCart;

public record ProductCartRequest(int UserId, List<ProductCartItemRequest> ProductCartItems);
