using EcommerceApp.Host.DTOs.ProductCart;
using ProductsApp.Domain.Products;

namespace EcommerceApp.Host.Services
{
    public static class ProductCartMappingExtensions
    {
        public static ProductCartResponse MapToProductCartResponse(this ProductCart productCart)
        {
            return new ProductCartResponse(
                productCart.Id,
                productCart.UserId,
                productCart.CartItems.Select(i => new ProductCartItemResponse(
                    i.ProductId,
                    i.Product.Name,
                    i.Quantity
                )).ToList());
        }

        public static ProductCartItem MapToCartItem(this ProductCartItemRequest request)
        {
            return new ProductCartItem
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };
        }

        public static ProductCart MapToProductCart(this ProductCartResponse response)
        {
            return new ProductCart
            {
                Id = response.Id,
                UserId = response.UserId,
                CartItems = response.ProductCartItems.Select(x => new ProductCartItem
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                }).ToList()
            };
        }
    }
}
