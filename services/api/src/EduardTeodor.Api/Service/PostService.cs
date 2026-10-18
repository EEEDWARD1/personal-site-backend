using EduardTeodor.Api.DTOs;
using EduardTeodor.Api.DTOs.Posts;
using EduardTeodor.Api.DTOs.Tags;
using EduardTeodor.Api.Models;
using EduardTeodor.Api.Repositories;

namespace EduardTeodor.Api.Service;

public class PostService(IPostRepository repository) : IPostService
{
    private readonly IPostRepository _repository = repository;

    public async Task<PostDetailedResponse?> GetPublishedPostBySlugAsync(string slug)
    {
        var post = await _repository.GetPublishedPostBySlugAsync(slug);

        return post is null ? null : new PostDetailedResponse(
            post.Slug,
            post.Title,
            post.Summary,
            post.Content,
            post.PublishedAt!.Value,
            post.Featured,
            MapTags(post.Tags));
    }

    public async Task<PagedResponse<PostSummaryResponse>> GetPublishedPostSummariesAsync(PostQuery query, PostType? type)
    {
        var (posts, page, pageSize, totalCount) = await GetPublishedPageAsync(query, type);

        var items = posts.Select(p => new PostSummaryResponse(
            p.Slug,
            p.Title,
            p.Summary,
            p.Featured,
            query.EnableTags is true ? MapTags(p.Tags) : null)).ToList();

        return new PagedResponse<PostSummaryResponse>(items, page, pageSize, totalCount);
    }

    public async Task<PagedResponse<PostTitleResponse>> GetPublishedPostTitlesAsync(PostQuery query, PostType? type)
    {
        var (posts, page, pageSize, totalCount) = await GetPublishedPageAsync(query, type);

        var items = posts.Select(p => new PostTitleResponse(
            p.Slug,
            p.Title,
            p.Featured,
            query.EnableTags is true ? MapTags(p.Tags) : null)).ToList();

        return new PagedResponse<PostTitleResponse>(items, page, pageSize, totalCount);
    }

    private async Task<(IReadOnlyList<PostListItem> Items, int Page, int PageSize, int TotalCount)> GetPublishedPageAsync(
        PostQuery query,
        PostType? type)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var skip = (int)Math.Min(((long)page - 1) * pageSize, int.MaxValue);

        var tagSlugs = query.Tag?
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .Distinct()
            .ToArray() ?? [];

        var (items, totalCount) = await _repository.GetPublishedPageAsync(
            skip, pageSize, type, query.IsFeatured, tagSlugs, query.EnableTags is true);

        return (items, page, pageSize, totalCount);
    }

    private static List<TagResponse> MapTags(IEnumerable<Tag> tags) =>
        tags.Select(t => new TagResponse(t.Name, t.Slug)).ToList();
}
