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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{   
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DevSeeder.SeedAsync(db);

    app.MapOpenApi();
    app.MapScalarApiReference();
}

var v1 = app.MapGroup("/api/v1");

v1.MapPostEndpoints();

app.UseHttpsRedirection();

app.Run();
