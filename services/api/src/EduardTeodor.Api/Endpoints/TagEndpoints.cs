using EduardTeodor.Api.DTOs;
using EduardTeodor.Api.DTOs.Tags;
using Microsoft.AspNetCore.Http.HttpResults;
using EduardTeodor.Api.Repositories;
using EduardTeodor.Api.Service;

namespace EduardTeodor.Api.Endpoints;

public static class TagEndpoints
{
    public static void MapTagEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/tags").WithTags("Tags");

        group.MapGet("/", GetTags).WithName("Get All Tags");
    }

    private static async Task<Ok<PagedResponse<TagResponse>>> GetTags(
        [AsParameters] TagQuery query,
        ITagService service)
    {
        var tags = await service.GetTagsAsync(query);
        return TypedResults.Ok(tags);
    }
}
