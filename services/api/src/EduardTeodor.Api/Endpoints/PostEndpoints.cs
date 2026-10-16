using EduardTeodor.Api.DTOs.Requests.Posts;
using EduardTeodor.Api.DTOs.Responses.Posts;
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
    }

    private static async Task<Ok<List<PostTitleResponse>>> GetPublishedPostTitles(
        [AsParameters] PostQuery query,
        IPostRepository repository)
    {
        var titles = await repository.GetPublishedPostTitlesAsync(query);

        return TypedResults.Ok(titles);
    }
}