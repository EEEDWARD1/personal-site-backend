using EduardTeodor.Api.Data;
using EduardTeodor.Api.DTOs;
using EduardTeodor.Api.DTOs.Posts;
using EduardTeodor.Api.DTOs.Tags;
using EduardTeodor.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EduardTeodor.Api.Repositories;

public class PostRepository(AppDbContext db) : IPostRepository
{
    public async Task<PostDetailedResponse?> GetPublishedPostBySlugAsync(string slug)
    {
        return await db.Posts
            .Where(p => p.Slug == slug
                    && p.PublishedAt != null && p.PublishedAt <= DateTime.UtcNow)
            .Select(p => new PostDetailedResponse(
                p.Slug,
                p.Title,
                p.Summary,
                p.Content,
                p.PublishedAt!.Value,
                p.Featured,
                p.Tags.Select(t => new TagResponse(t.Name, t.Slug)).ToList()
            )).FirstOrDefaultAsync();
    }

    public async Task<PagedResponse<PostSummaryResponse>> GetPublishedPostSummariesAsync(PostQuery query, PostType? type)
    {
        var posts = ApplyPostFilters(
            db.Posts.Where(p => p.PublishedAt != null && p.PublishedAt <= DateTime.UtcNow),
            query,
            type);

        var totalCount = await posts.CountAsync();
        var (page, pageSize, skip) = GetPagination(query);

        var orderedPosts = posts
            .OrderByDescending(p => p.PublishedAt)
            .ThenBy(p => p.Id)
            .Skip(skip)
            .Take(pageSize);

        var items = query.EnableTags is true
            ? await orderedPosts
                .Select(p => new PostSummaryResponse(
                    p.Slug,
                    p.Title,
                    p.Summary,
                    p.Featured,
                    p.Tags.Select(t => new TagResponse(t.Name, t.Slug)).ToList()))
                .ToListAsync()
            : await orderedPosts
                .Select(p => new PostSummaryResponse(p.Slug, p.Title, p.Summary, p.Featured))
                .ToListAsync();

        return new PagedResponse<PostSummaryResponse>(items, page, pageSize, totalCount);
    }

    public async Task<PagedResponse<PostTitleResponse>> GetPublishedPostTitlesAsync(PostQuery query, PostType? type)
    {
        var posts = ApplyPostFilters(
            db.Posts.Where(p => p.PublishedAt != null && p.PublishedAt <= DateTime.UtcNow),
            query,
            type);

        var totalCount = await posts.CountAsync();
        var (page, pageSize, skip) = GetPagination(query);

        var orderedPosts = posts
            .OrderByDescending(p => p.PublishedAt)
            .ThenBy(p => p.Id)
            .Skip(skip)
            .Take(pageSize);

        var items = query.EnableTags is true
            ? await orderedPosts
                .Select(p => new PostTitleResponse(
                    p.Slug,
                    p.Title,
                    p.Featured,
                    p.Tags.Select(t => new TagResponse(t.Name, t.Slug)).ToList()))
                .ToListAsync()
            : await orderedPosts
                .Select(p => new PostTitleResponse(p.Slug, p.Title, p.Featured))
                .ToListAsync();

        return new PagedResponse<PostTitleResponse>(items, page, pageSize, totalCount);
    }

    private static (int Page, int PageSize, int Skip) GetPagination(PostQuery query)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var skip = (int)Math.Min(((long)page - 1) * pageSize, int.MaxValue);

        return (page, pageSize, skip);
    }

    private static IQueryable<Post> ApplyPostFilters(IQueryable<Post> posts, PostQuery query, PostType? type)
    {
        if (type is { } postType)
            posts = posts.Where(p => p.Type == postType);

        if (query.IsFeatured.HasValue)
            posts = posts.Where(p => p.Featured == query.IsFeatured.Value);

        var tagSlugs = query.Tag?
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .Distinct()
            .ToArray();

        if (tagSlugs is { Length: > 0 })
            posts = posts.Where(p => p.Tags.Any(tag => tagSlugs.Contains(tag.Slug)));

        return posts;
    }
}
