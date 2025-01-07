namespace Products.Api.TargetingFeature.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public bool Discounted { get; set; }
    public decimal DiscountPercentage { get; set; }
    public int Quantity { get; set; }
}
