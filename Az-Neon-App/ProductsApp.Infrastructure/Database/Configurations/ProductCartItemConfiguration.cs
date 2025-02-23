using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsApp.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsApp.Infrastructure.Database.Configurations;

public class ProductCartItemConfiguration : IEntityTypeConfiguration<ProductCartItem>
{
    public void Configure(EntityTypeBuilder<ProductCartItem> builder)
    {
        builder.HasKey(pci=>pci.Id);

        builder.Property(pci=>pci.Id)
            .ValueGeneratedOnAdd();

        builder.HasOne(pci=>pci.ProductCart)
            .WithMany(pc=>pc.CartItems)
            .HasForeignKey(pc=>pc.ProductCartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pci=>pci.Product)
            .WithMany()
            .HasForeignKey(pci=>pci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(pci => pci.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

    }
}
