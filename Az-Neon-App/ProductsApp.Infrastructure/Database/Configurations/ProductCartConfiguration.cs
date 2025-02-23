using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsApp.Domain.Products;

namespace ProductsApp.Infrastructure.Database.Configurations;

public class ProductCartConfiguration : IEntityTypeConfiguration<ProductCart>
{
    public void Configure(EntityTypeBuilder<ProductCart> builder)
    {
        builder.HasKey(pc=>pc.Id);

        builder.Property(pc => pc.Id)
        .HasColumnType("uuid")
        .HasDefaultValueSql("gen_random_uuid()");   
              

        builder.HasOne(pc=>pc.User)
               .WithMany()
               .HasForeignKey(pc=>pc.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(pc => pc.CreatedOn);
    }
}
