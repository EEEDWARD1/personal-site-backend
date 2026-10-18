using EduardTeodor.Api.Data;
using EduardTeodor.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EduardTeodor.Api.Repositories;

public class PostRepository(AppDbContext db) : IPostRepository
{
    public async Task<Post?> GetPublishedPostBySlugAsync(string slug)
    {
        return await db.Posts
            .AsNoTracking()
            .Include(p => p.Tags)
            .Where(p => p.Slug == slug
                    && p.PublishedAt != null && p.PublishedAt <= DateTime.UtcNow)
            .FirstOrDefaultAsync();
    }

    public async Task<(IReadOnlyList<PostListItem> Items, int TotalCount)> GetPublishedPageAsync(
        int skip,
        int take,
        PostType? type,
        bool? isFeatured,
        string[] tagSlugs,
        bool includeTags)
    {
        var posts = ApplyPostFilters(
            db.Posts.AsNoTracking()
                .Where(p => p.PublishedAt != null && p.PublishedAt <= DateTime.UtcNow),
            type,
            isFeatured,
            tagSlugs);

        var totalCount = await posts.CountAsync();

        var orderedPosts = posts
            .OrderByDescending(p => p.PublishedAt)
            .ThenBy(p => p.Id)
            .Skip(skip)
            .Take(take);

        var items = includeTags
            ? await orderedPosts
                .Select(p => new PostListItem(
                    p.Slug,
                    p.Title,
                    p.Summary,
                    p.Featured,
                    p.Tags.ToList()))
                .ToListAsync()
            : await orderedPosts
                .Select(p => new PostListItem(p.Slug, p.Title, p.Summary, p.Featured, new List<Tag>()))
                .ToListAsync();

        return (items, totalCount);
    }

    private static IQueryable<Post> ApplyPostFilters(
        IQueryable<Post> posts,
        PostType? type,
        bool? isFeatured,
        string[] tagSlugs)
    {
        if (type is { } postType)
            posts = posts.Where(p => p.Type == postType);

        if (isFeatured.HasValue)
            posts = posts.Where(p => p.Featured == isFeatured.Value);

        if (tagSlugs is { Length: > 0 })
            posts = posts.Where(p => p.Tags.Any(tag => tagSlugs.Contains(tag.Slug)));

        return posts;
    }
}
