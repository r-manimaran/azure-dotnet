using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

using var log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log.txt")
                .CreateLogger();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
