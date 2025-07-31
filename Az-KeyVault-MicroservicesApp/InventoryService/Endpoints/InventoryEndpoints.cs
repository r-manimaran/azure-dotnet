using Carter;

namespace InventoryService.Endpoints;

public class InventoryEndpoints : CarterModule
{
    public InventoryEndpoints() :base("api/inventory")
    {
        this.IncludeInOpenApi();
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("config", (IConfiguration configurations) =>
        {
            var connection = configurations["Connection"];
            var apiEndPoint = configurations["ApiEndpoint"];

            return Results.Ok(new
            {
                Connection = connection ?? "Connection not found",
                ApiEndpoint = apiEndPoint ?? "ApiEndpoint not found"
            });
        });
    }
}
