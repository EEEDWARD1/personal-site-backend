using ResourceApi.DTOs;
using ResourceApi.Models;
using ResourceApi.Repositories;

namespace ResourceApi.Endpoints;

public static class PostEndpoints
{
    public static void MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/posts").WithTags("Posts");

        group.MapGet("/titles", GetPostTitles)
            .WithName("GetPostTitles")
            .WithSummary("Get published post titles")
            .WithDescription("Optionally filter post titles by kind.")
            .Produces<List<PostTitleDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/", GetPostSummaries)
            .WithName("GetPostSummaries")
            .WithSummary("Get published post summaries")
            .WithDescription("Optionally filter post titles by kind.")
            .Produces<List<PostSummaryDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/{slug}", GetPostBySlug)
            .WithName("GetPostBySlug")
            .WithSummary("Get a published post by its slug")
            .WithDescription("Returns the full content and tags of a published post.")
            .Produces<PostDetailedDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreatePost)
            .WithName("CreatePost")
            .WithSummary("Create a new post")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetPostTitles(
        bool? isProject,
        bool? isFeatured,
        IPostRepository repository)
    {
        var titles = await repository.GetPostTitlesAsync(isProject, isFeatured);

        return Results.Ok(titles);
    }

    private static async Task<IResult> GetPostSummaries(
        bool? isProject,
        bool? isFeatured,
        IPostRepository repository)
    {
       
        var summary = await repository.GetPostSummaryAsync(isProject, isFeatured);
        
        return Results.Ok(summary);
    }

    private static async Task<IResult> GetPostBySlug(
        string slug,
        IPostRepository repository)
    {
        var post = await repository.GetPostBySlugAsync(slug);

        return post is null
            ? Results.NotFound()
            : Results.Ok(post);
    }

    private static async Task<IResult> CreatePost(
        CreatePostDto request,
        IPostRepository repository)
    {
        var slug = await repository.CreatePostAsync(request);

        return Results.Json(
        new
        {
            slug,
            published = request.Published
        },
        statusCode: StatusCodes.Status201Created);
    }
}