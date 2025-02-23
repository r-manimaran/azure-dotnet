using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsApp.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsApp.Infrastructure.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(u => u.Id)
               .ValueGeneratedOnAdd();

        builder.Property(u => u.Username)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(u=>u.Email)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(u => u.Email)
               .IsUnique();


    }
}
