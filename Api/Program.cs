using Api.Data;
using Api.Endpoints;
using Api.Entities;
using Api.Services;
using DbUp;
using DbUp.Postgresql;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured.");

// 1. Run SQL migrations (DbUp) before the app starts serving requests
var upgrader = DeployChanges.To
    .PostgresqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(typeof(Program).Assembly)
    .LogToConsole()
    .Build();

var migration = upgrader.PerformUpgrade();
if (!migration.Successful)
{
    Console.Error.WriteLine(migration.Error);
    return 1;
}

// 2. Database access (EF Core is only a mapper; the SQL files own the schema)
builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseNpgsql(connectionString, npgsql => npgsql.MapEnum<PostKind>("post_kind"))
        .UseSnakeCaseNamingConvention());

// 3. Application services (one instance per HTTP request)
builder.Services.AddScoped<PostService>();

// 4. OpenAPI document generation (Scalar renders it as docs)
builder.Services.AddOpenApi();

var app = builder.Build();

// 5. API docs, development only
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();            // JSON spec at /openapi/v1.json
    app.MapScalarApiReference(); // UI at /scalar/v1
}

// 6. Endpoints
app.MapPostEndpoints();

app.Run();
return 0;