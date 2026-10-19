using System.ComponentModel.DataAnnotations;
using EduardTeodor.Api.Data;
using EduardTeodor.Api.DTOs.Posts;
using EduardTeodor.Api.Endpoints;
using EduardTeodor.Api.Health;
using EduardTeodor.Api.Models;
using EduardTeodor.Api.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EduardTeodor.Api.Tests;

public class ReadinessAndRouteTests
{
    [Fact]
    public async Task DatabaseHealthIsUnhealthyWhenPostgresCannotBeReached()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=127.0.0.1;Port=1;Database=unavailable;Username=test;Password=test;Timeout=1;Pooling=false")
            .Options;
        await using var db = new AppDbContext(options);
        var check = new DatabaseHealthCheck(db);

        var result = await check.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.NotNull(result.Exception);
        Assert.Equal("Database readiness check failed.", result.Description);
    }

    [Fact]
    public async Task EndpointContractUsesOnlyV1Prefix()
    {
        var builder = WebApplication.CreateBuilder();
        // Only metadata is inspected; these services must not be invoked.
        builder.Services.AddScoped<IPostService>(_ => throw new InvalidOperationException());
        builder.Services.AddScoped<ITagService>(_ => throw new InvalidOperationException());
        builder.Services.AddScoped<IAdminPostService>(_ => throw new InvalidOperationException());
        await using var app = builder.Build();
        var group = app.MapGroup("/v1");
        group.MapPostEndpoints();
        group.MapTagEndpoints();
        group.MapAdminEndpoints();

        var paths = ((IEndpointRouteBuilder)app).DataSources.SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>().Select(endpoint => endpoint.RoutePattern.RawText!.TrimEnd('/')).ToList();

        Assert.NotEmpty(paths);
        Assert.All(paths, path => Assert.StartsWith("/v1/", path));
        Assert.Contains("/v1/posts", paths);
        Assert.Contains("/v1/posts/titles", paths);
        Assert.Contains("/v1/posts/{slug}", paths);
        Assert.Contains("/v1/tags", paths);
        Assert.Contains("/v1/admin/posts", paths);
        Assert.Contains("/v1/admin/posts/{id:guid}", paths);
        Assert.DoesNotContain(paths, path => path.StartsWith("/api/"));
    }

    [Theory]
    [InlineData("titles")]
    [InlineData("TITLES")]
    public void BothWriteRequestsRejectReservedSlug(string slug)
    {
        object[] requests =
        [
            new CreatePostRequest(slug, "Title", "Content"),
            new UpdatePostRequest(slug, "Title", "Content", "", PostType.Thought, false, null)
        ];
        foreach (var request in requests)
        {
            var errors = new List<ValidationResult>();
            Assert.False(Validator.TryValidateObject(request, new ValidationContext(request), errors, true));
            Assert.Contains(errors, error => error.MemberNames.Contains("Slug")
                && error.ErrorMessage!.Contains("reserved"));
        }
    }

    [Fact]
    public void SlugsContainingTitlesRemainValid()
    {
        var request = new CreatePostRequest("my-titles", "Title", "Content");
        Assert.True(Validator.TryValidateObject(request, new ValidationContext(request), [], true));
    }
}
