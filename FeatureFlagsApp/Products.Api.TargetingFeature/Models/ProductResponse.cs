namespace Products.Api.TargetingFeature.Models;

//V1 Response
public sealed record ProductResponseV1
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
}

// V2 Response

public sealed record ProductResponseV2
{
    public Guid Id { get; init; }
    public ProductInfoV2 Product { get; init; }
    public InventoryInfoV2 Inventory { get; init; }
}

public sealed record ProductInfoV2
{
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public PricingInfoV2 Pricing { get; init; }  
}

public sealed record PricingInfoV2
{
    public decimal Amount { get; init; }
    public string Currenncy { get; init; } = string.Empty;
    public bool Discounted { get; init; } = false;
}

public sealed record InventoryInfoV2
{
    public bool InStock { get; init; } = false;
    public int Quantity { get; init; } = 0;
}