using Api.DTOs;
using Api.Entities;
using Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Endpoints;

public static class PostEndpoints
{
    public static void MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        MapPublicGroup(app, "/v1/projects", PostKind.Project, "PROJECT_NOT_FOUND", "Project was not found.");
        MapPublicGroup(app, "/v1/thoughts", PostKind.Thought, "THOUGHT_NOT_FOUND", "Thought was not found.");
    }

    private static void MapPublicGroup(
        IEndpointRouteBuilder app, string prefix, PostKind kind, string notFoundCode, string notFoundMessage)
    {
        var group = app.MapGroup(prefix);

        group.MapGet("/{slug}", async Task<Results<Ok<PublicPostResponse>, NotFound<ApiError>>> (
            string slug, PostService posts, CancellationToken ct) =>
        {
            var post = await posts.GetPublishedBySlugAsync(kind, slug, ct);

            return post is null
                ? TypedResults.NotFound(new ApiError(new ErrorBody(notFoundCode, notFoundMessage)))
                : TypedResults.Ok(post);
        });
    }
}