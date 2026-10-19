using System.Text.Json;
using System.Text.Json.Serialization;
using EduardTeodor.Api.Data;
using EduardTeodor.Api.Endpoints;
using EduardTeodor.Api.Repositories;
using EduardTeodor.Api.Service;
using EduardTeodor.Api.Authentication;
using EduardTeodor.Api.Health;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<AppDbContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
    .UseSnakeCaseNamingConvention()
);

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IAdminPostRepository, AdminPostRepository>();
builder.Services.AddScoped<IAdminPostService, AdminPostService>();
builder.Services.AddClerkAuthentication(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>(
    "database", timeout: TimeSpan.FromSeconds(5));

builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));
        
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{   
    app.MapOpenApi();
    app.MapScalarApiReference();
    
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DevSeeder.SeedAsync(db);
}

var v1 = app.MapGroup("/v1");
v1.MapPostEndpoints();
v1.MapTagEndpoints();
v1.MapAdminEndpoints();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.Headers.CacheControl = "no-store";
        // Do not expose connection details or exception messages publicly.
        await context.Response.WriteAsJsonAsync(new
        {
            status = report.Status == HealthStatus.Healthy ? "healthy" : "unhealthy"
        });
    }
})
    .WithName("Health");

app.Run();
