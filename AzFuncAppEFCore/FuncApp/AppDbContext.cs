using FuncApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncApp;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
       
    }
    // Define DbSets for your entities here
   public DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured)
        {
           // _logger.LogInformation("Configuring the DbContext with connection string from environment variable.");
            var connectionString = Environment.GetEnvironmentVariable("Sqldb");
            if (!string.IsNullOrEmpty(connectionString))
            {
             //   _logger.LogInformation($"Connection string found in environment variable.: {connectionString}");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
      //  modelBuilder.Entity<Student>().HasData(
        //    new Student { Id = 4, FirstName = "Tom", LastName = "Hanks", Email = "Tom@email.com" });
    }
}
