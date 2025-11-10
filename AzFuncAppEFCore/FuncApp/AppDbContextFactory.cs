using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FuncApp;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("Sqldb") ?? 
            "Server=tcp:marandbserver.database.windows.net,1433;Initial Catalog=demodb;User ID=demoadmin;Password=password;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        
        optionsBuilder.UseSqlServer(connectionString);
        return new AppDbContext(optionsBuilder.Options);
    }
}