using EmployeesApi.Endpoints;
using EmployeesApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.AddAzureTableClient("strTables");

builder.Services.AddScoped<IAzStorageTablesService, AzStorageTablesService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint(
        "/openapi/v1.json", "OpenAPI v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Add the Employee Endpoints to the application
app.MapEmployeeEndpoints();

app.Run();
