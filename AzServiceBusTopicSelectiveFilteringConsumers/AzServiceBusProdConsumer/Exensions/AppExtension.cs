using Microsoft.EntityFrameworkCore;

namespace AzServiceBusProdConsumer.Exensions;

public static class AppExtension
{
    public static async Task ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // Check if there are any pending Migrations
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            // Apply the pending Migrations
            await dbContext.Database.MigrateAsync();
        }
    }
}
