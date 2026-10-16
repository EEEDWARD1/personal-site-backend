using EduardTeodor.Api.DTOs;
using EduardTeodor.Api.DTOs.Posts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EduardTeodor.Api.Endpoints;

public static class PostEndpoints
{
    public static void MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/posts").WithTags("Posts");

        group.MapGet("/titles", GetPublishedPostTitles)
            .WithName("Get Published Post Titles");

        group.MapGet("/", GetPublishedPostSummaries)
            .WithName("Get Published Post Summaries");

        group.MapGet("/{slug}", GetPublishedPostBySlug)
            .WithName("Get Published Post By Slug");
    }

    private static async Task<Results<Ok<PagedResponse<PostTitleResponse>>, BadRequest<ProblemDetails>>> GetPublishedPostTitles(
        [AsParameters] PostQuery query,
        IPostRepository repository)
    {
        if (query.Type is { IsValid: false } invalidType)
            return TypedResults.BadRequest(CreateInvalidTypeProblem(invalidType.Input));

        var titles = await repository.GetPublishedPostTitlesAsync(query);

        return TypedResults.Ok(titles);
    }

    private static async Task<Results<Ok<PagedResponse<PostSummaryResponse>>, BadRequest<ProblemDetails>>> GetPublishedPostSummaries(
        [AsParameters] PostQuery query,
        IPostRepository repository)
    {
        if (query.Type is { IsValid: false } invalidType)
            return TypedResults.BadRequest(CreateInvalidTypeProblem(invalidType.Input));

        var summaries = await repository.GetPublishedPostSummariesAsync(query);

        return TypedResults.Ok(summaries);
    }

    private static ProblemDetails CreateInvalidTypeProblem(string? input) => new()
    {
        Status = StatusCodes.Status400BadRequest,
        Title = "Invalid post type",
        Detail = $"'{input}' is not a valid post type. Allowed values are 'project' and 'thought'."
    };
    
    private static async Task<Results<Ok<PostDetailedResponse>, NotFound>> GetPublishedPostBySlug(
        string slug,
        IPostRepository repository)
    {
        var post = await repository.GetPublishedPostBySlugAsync(slug);

        return post is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(post);
    }
}
