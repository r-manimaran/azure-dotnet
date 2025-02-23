using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsApp.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsApp.Infrastructure.Database.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p=>p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Name).IsRequired().HasMaxLength(150);

        builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");

        builder.Property(p => p.Description).HasMaxLength(500);
    }
}
