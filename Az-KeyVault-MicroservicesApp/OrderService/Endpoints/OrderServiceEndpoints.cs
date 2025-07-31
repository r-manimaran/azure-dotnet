using Microsoft.AspNetCore.Http.HttpResults;

namespace OrderService.Endpoints;

public static class OrderServiceEndpoints
{
    public static void MapOrderServiceEndpoints(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("/api/orderservice").WithTags("OrderService").WithOpenApi();

        api.MapGet("connection", (IConfiguration configuration) =>
        {
            string connection = configuration["Connection"] ?? "Connection not found";
            return Results.Ok(new
            {
                Connection = connection
            });
        });
    }
}
