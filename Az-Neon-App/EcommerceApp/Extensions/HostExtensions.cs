using EcommerceApp.Host.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductsApp.Domain.Products;
using ProductsApp.Infrastructure.Database;
using ProductsApp.Infrastructure.Repositories;

namespace EcommerceApp.Host.Extensions;

public static class HostExtensions
{
    public static IServiceCollection AddWebHostInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IProductCartRepository, ProductCartRepository>();

        services.AddEfCore(configuration);
        return services;
    }

    public static IServiceCollection AddEfCore(this IServiceCollection services, 
                                               IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");
        services.AddDbContext<AppDbContext>((_, options) =>
        {
            options.EnableSensitiveDataLogging()
                .UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable(DatabaseConst.MigrationHistoryTable, DatabaseConst.Schema);
                });
            options.UseSnakeCaseNamingConvention();
        });

        return services;
    }

    public static async Task ApplyMigrations(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {

            // To create migrations run the command:
            // dotnet ef migrations add initialCreate --project .\ProductsApp.Infrastructure\ProductsApp.Infrastructure.csproj --startup-project .\EcommerceApp\EcommerceApp.Host.csproj
            // dotnet ef migrations add Initial --startup-project ./ProductService.Host --project ./ProductService.Infrastructure -- --context ProductService.Infrastructure.Database.ApplicationDbContext
            await using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                await dbContext.Database.MigrateAsync();
                await DatabaseSeedService.SeedAsync(dbContext);
            }
                          
        }
    }
}
