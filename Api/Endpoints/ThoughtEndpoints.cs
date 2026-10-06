using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Endpoints;

public static class ThoughtEndpoints
{
    public static void MapThoughtEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/thoughts");

        group.MapGet("/{slug}", GetBySlug);
    }

    private static async Task<Results<Ok<ThoughtResponse>, NotFound<ApiError>>> GetBySlug(
        string slug, ThoughtService thoughts, CancellationToken ct)
    {
        var thought = await thoughts.GetBySlugAsync(slug, ct);

        return thought is null
            ? TypedResults.NotFound(new ApiError(
                new ErrorBody("THOUGHT_NOT_FOUND", "Thought was not found.")))
            : TypedResults.Ok(thought);
    }
}