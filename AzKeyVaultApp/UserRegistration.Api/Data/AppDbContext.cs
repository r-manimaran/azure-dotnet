
using Microsoft.EntityFrameworkCore;
using UserRegistration.Api.Data.Configurations;
using UserRegistration.Api.Models;

namespace UserRegistration.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
        
    }

    public DbSet<Registration> Registrations { get; set; }

    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new RegistrationConfiguration());
    }
}
