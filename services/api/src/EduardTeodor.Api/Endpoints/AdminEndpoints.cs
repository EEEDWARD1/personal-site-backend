using System.ComponentModel.DataAnnotations;
using System.Globalization;
using EduardTeodor.Api.DTOs;
using EduardTeodor.Api.DTOs.Posts;
using EduardTeodor.Api.Models;
using EduardTeodor.Api.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EduardTeodor.Api.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin").WithTags("Admin").RequireAuthorization("Admin");

        group.MapGet("/posts", GetPostTitles)
            .WithName("Get Post Titles including drafts");

        group.MapPost("/posts", Create)
            .WithName("Create Post");
        group.MapDelete("/posts/{id:guid}", Delete)
            .WithName("Delete Post");
        group.MapGet("/posts/{id:guid}", GetById)
            .WithName("Get Admin Post By Id");
        group.MapPut("/posts/{id:guid}", Update)
            .WithName("Update Post");
    }

    private static async Task<Ok<PagedResponse<AdminPostTitleResponse>>> GetPostTitles(
        [AsParameters] PageQuery query,
        IAdminPostService service) => TypedResults.Ok(await service.GetTitlesAsync(query));

    private static async Task<Results<Ok<AdminPostDetailedResponse>, NotFound>> GetById(
        Guid id, HttpContext httpContext, IAdminPostService service)
    {
        var result = await service.GetByIdAsync(id);
        if (result is null)
            return TypedResults.NotFound();

        httpContext.Response.Headers.ETag = $"\"{result.Value.Version}\"";
        httpContext.Response.Headers.CacheControl = "no-store";
        return TypedResults.Ok(result.Value.Post);
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem, ProblemHttpResult>> Update(
        Guid id, UpdatePostRequest request, HttpContext httpContext, IAdminPostService service)
    {
        var errors = Validate(request);
        if (errors.Count > 0)
            return TypedResults.ValidationProblem(errors);

        var ifMatch = httpContext.Request.Headers.IfMatch.ToString();
        if (string.IsNullOrEmpty(ifMatch))
            return TypedResults.Problem(statusCode: StatusCodes.Status428PreconditionRequired,
                title: "If-Match is required", detail: "Load the post and send its ETag in the If-Match header.");

        if (ifMatch.Length < 3 || ifMatch[0] != '"' || ifMatch[^1] != '"' ||
            !uint.TryParse(ifMatch.AsSpan(1, ifMatch.Length - 2), NumberStyles.None,
                CultureInfo.InvariantCulture, out var version))
            return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid If-Match", detail: "Send the single, quoted ETag returned by GET, for example: \"123\".");

        return await service.UpdateAsync(id, request, version) switch
        {
            PostUpdateStatus.Updated => TypedResults.NoContent(),
            PostUpdateStatus.NotFound => TypedResults.NotFound(),
            PostUpdateStatus.VersionConflict => TypedResults.Problem(
                statusCode: StatusCodes.Status412PreconditionFailed,
                title: "Post has changed", detail: "Reload the post before saving your changes."),
            PostUpdateStatus.DuplicateSlug => TypedResults.Problem(
                statusCode: StatusCodes.Status409Conflict, title: "Post slug already exists"),
            _ => throw new InvalidOperationException("Unexpected post update status.")
        };
    }

    private static Dictionary<string, string[]> Validate(object request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);
        return results.SelectMany(result => result.MemberNames.Select(name =>
                (Name: name, Message: result.ErrorMessage ?? "Invalid value.")))
            .GroupBy(error => error.Name)
            .ToDictionary(group => group.Key, group => group.Select(error => error.Message).ToArray());
    }

    private static async Task<Results<Created<AdminPostTitleResponse>, ValidationProblem, Conflict<ProblemDetails>>> Create(
        CreatePostRequest request,
        HttpContext httpContext,
        IAdminPostService service)
    {
        var errors = Validate(request);
        if (errors.Count > 0)
            return TypedResults.ValidationProblem(errors);

        var post = await service.CreateAsync(request);
        if (post is null)
            return TypedResults.Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Post slug already exists"
            });

        return TypedResults.Created($"{httpContext.Request.Path}/{post.Id}", post);
    }

    private static async Task<Results<NoContent, NotFound>> Delete(Guid id, IAdminPostService service) =>
        await service.DeleteAsync(id) ? TypedResults.NoContent() : TypedResults.NotFound();
}
