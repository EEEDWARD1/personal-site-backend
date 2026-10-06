using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/projects");

        group.MapGet("/{slug}", GetBySlug);
    }

    private static async Task<Results<Ok<ProjectResponse>, NotFound<ApiError>>> GetBySlug(
        string slug, ProjectService projects, CancellationToken ct)
    {
        var project = await projects.GetBySlugAsync(slug, ct);

        return project is null
            ? TypedResults.NotFound(new ApiError(
                new ErrorBody("PROJECT_NOT_FOUND", "Project was not found.")))
            : TypedResults.Ok(project);
    }
}