using EduardTeodor.Api.DTOs;
using EduardTeodor.Api.DTOs.Posts;
using EduardTeodor.Api.Models;
using EduardTeodor.Api.Service;
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
        IPostService service)
    {
        if (!TryParsePostType(query.Type, out var type))
            return TypedResults.BadRequest(CreateInvalidTypeProblem(query.Type));

        var titles = await service.GetPublishedPostTitlesAsync(query, type);

        return TypedResults.Ok(titles);
    }

    private static async Task<Results<Ok<PagedResponse<PostSummaryResponse>>, BadRequest<ProblemDetails>>> GetPublishedPostSummaries(
        [AsParameters] PostQuery query,
        IPostService service)
    {
        if (!TryParsePostType(query.Type, out var type))
            return TypedResults.BadRequest(CreateInvalidTypeProblem(query.Type));

        var summaries = await service.GetPublishedPostSummariesAsync(query, type);

        return TypedResults.Ok(summaries);
    }

    private static ProblemDetails CreateInvalidTypeProblem(string? input) => new()
    {
        Status = StatusCodes.Status400BadRequest,
        Title = "Invalid post type",
        Detail = $"'{input}' is not a valid post type. Allowed values are 'project' and 'thought'."
    };

    private static bool TryParsePostType(string? value, out PostType? type)
    {
        type = null;

        if (value is null)
            return true;

        var enumName = Enum.GetNames<PostType>()
            .FirstOrDefault(name => string.Equals(name, value.Trim(), StringComparison.OrdinalIgnoreCase));

        if (enumName is null)
            return false;

        type = Enum.Parse<PostType>(enumName);
        return true;
    }
    
    private static async Task<Results<Ok<PostDetailedResponse>, NotFound>> GetPublishedPostBySlug(
        string slug,
        IPostService service)
    {
        var post = await service.GetPublishedPostBySlugAsync(slug);

        return post is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(post);
    }
}
