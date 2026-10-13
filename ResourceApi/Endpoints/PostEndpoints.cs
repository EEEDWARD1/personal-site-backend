using Microsoft.EntityFrameworkCore;
using ResourceApi.Data;
using ResourceApi.DTOs;
using ResourceApi.Models;

namespace ResourceApi.Endpoints;

public static class PostEndpoints
{
    public static void MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/posts").WithTags("Posts");

        group.MapGet("/titles", async (string? kind, bool? isFeatured, IPostRepository repository) =>
        {
            PostKind? parsedKind = null;
            if (!string.IsNullOrWhiteSpace(kind))
            {
                if (!Enum.TryParse<PostKind>(kind, true, out var value)
                    || !Enum.IsDefined(value))
                {
                    return Results.BadRequest(
                        "Invalid post kind. Use 'project' or 'thought'.");
                }

                parsedKind = value;
            }
            var titles = await repository.GetPostTitlesAsync(parsedKind, isFeatured);

            return Results.Ok(titles);
        })
        .WithName("GetPostTitles")
        .WithSummary("Get published post titles")
        .WithDescription("Optionally filter post titles by kind.")
        .Produces<List<PostTitleDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    
        group.MapGet("/", async (string? kind, bool? isFeatured, IPostRepository repository) =>
        {
            PostKind? parsedKind = null;
                        if (!string.IsNullOrWhiteSpace(kind))
            {
                if (!Enum.TryParse<PostKind>(kind, true, out var value)
                    || !Enum.IsDefined(value))
                {
                    return Results.BadRequest(
                        "Invalid post kind. Use 'project' or 'thought'.");
                }

                parsedKind = value;
            }
            var summary = await repository.GetPostSummaryAsync(parsedKind, isFeatured);
            return Results.Ok(summary);
        })
        .WithName("GetPostSummaries")
        .WithSummary("Get published post summaries")
        .WithDescription("Optionally filter post titles by kind.")
        .Produces<List<PostTitleDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/{slug}", async (
        string slug,
        IPostRepository repository) =>
        {
            var post = await repository.GetPostBySlugAsync(slug);

            return post is null
                ? Results.NotFound()
                : Results.Ok(post);
        })
        .WithName("GetPostBySlug")
        .WithSummary("Get a published post by its slug")
        .WithDescription("Returns the full content and tags of a published post.")
        .Produces<PostDetailedDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}