using Bogus;
using Products.Api.TargetingFeature.Data;

namespace Products.Api.TargetingFeature.Extensions
{
    public static class DbSeederExtensions
    {
        static Faker _faker = new Faker();
        
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using(var scope = app.ApplicationServices.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<ProductSeeder>();
                await seeder.SeedAsync();
            }
        }
    }
}
