using System.Text.Json;
using System.Text.Json.Serialization;
using EduardTeodor.Api.Data;
using EduardTeodor.Api.Endpoints;
using EduardTeodor.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<AppDbContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
    .UseSnakeCaseNamingConvention()
);

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));
        
var app = builder.Build();

if (app.Environment.IsDevelopment())
{   
    app.MapOpenApi();
    app.MapScalarApiReference();
    
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DevSeeder.SeedAsync(db);
}

var v1 = app.MapGroup("/api/v1");
v1.MapPostEndpoints();
v1.MapTagEndpoints();

app.Run();
