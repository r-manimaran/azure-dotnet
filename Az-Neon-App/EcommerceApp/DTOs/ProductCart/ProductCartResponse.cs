namespace EcommerceApp.Host.DTOs.ProductCart;

public record ProductCartResponse(Guid Id, int UserId, List<ProductCartItemResponse> ProductCartItems);
