using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsApp.Domain.Products;

public class ProductCartItem
{
    public int Id { get; set; }
    public Guid ProductCartId { get; set; }
    public ProductCart ProductCart { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
}
