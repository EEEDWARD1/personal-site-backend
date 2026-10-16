using EduardTeodor.Api.DTOs;
using EduardTeodor.Api.DTOs.Posts;
using EduardTeodor.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

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

    private static async Task<Ok<PagedResponse<PostTitleResponse>>> GetPublishedPostTitles(
        [AsParameters] PostQuery query,
        IPostRepository repository)
    {
        var titles = await repository.GetPublishedPostTitlesAsync(query);

        return TypedResults.Ok(titles);
    }

    private static async Task<Ok<PagedResponse<PostSummaryResponse>>> GetPublishedPostSummaries(
        [AsParameters] PostQuery query,
        IPostRepository repository)
    {
        var summaries = await repository.GetPublishedPostSummariesAsync(query);

        return TypedResults.Ok(summaries);
    }
    
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
