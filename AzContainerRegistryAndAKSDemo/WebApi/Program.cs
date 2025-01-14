using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Dtos;
using WebApi.Services;
using WebApi.Validations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

/*builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName ="test";
});*/

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureSql"));
});

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddValidatorsFromAssemblyContaining<CreatePostValidator>();

builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddScoped<IPostService, PostService>();

var app = builder.Build();

app.MapOpenApi();

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
