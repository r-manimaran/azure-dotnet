using AspireApp.AzureBlob.ApiService;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddAzureBlobClient("blobs");

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

builder.Services.AddScoped<IContainerService, ContainerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();

app.UseSwaggerUI();

app.MapControllers();

app.MapDefaultEndpoints();

app.Run();

