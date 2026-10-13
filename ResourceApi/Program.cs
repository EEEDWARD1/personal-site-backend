using Microsoft.EntityFrameworkCore;
using ResourceApi.Data;
using ResourceApi.Endpoints;
using ResourceApi.Models;
using ResourceApi.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<AppDbContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("AppDbContext"))
    .UseSnakeCaseNamingConvention()
);

builder.Services.AddOpenApi();
builder.Services.AddScoped<IPostRepository, PostRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

if (app.Environment.IsDevelopment() && args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
    await DatabaseSeeder.SeedAsync(db);
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World");
app.MapPostEndpoints();

app.Run();
