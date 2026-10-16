using EduardTeodor.Api.Data;
using EduardTeodor.Api.DTOs.Posts;
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
                p.Featured
            )).FirstOrDefaultAsync();
    }

    public async Task<List<PostSummaryResponse>> GetPublishedPostSummariesAsync(PostQuery query)
    {
        var posts = db.Posts
            .Where(p => p.PublishedAt != null && p.PublishedAt <= DateTime.UtcNow);
        
        if (query.Type is { } type)
            posts = posts.Where(p => p.Type == type);

        if (query.IsFeatured.HasValue)
            posts = posts.Where(p => p.Featured == query.IsFeatured.Value);
        
        return await posts
            .OrderByDescending(p => p.PublishedAt)
            .Select(p => new PostSummaryResponse(
                p.Slug, 
                p.Title, 
                p.Summary, p.Featured))
            .ToListAsync();
    }

    public async Task<List<PostTitleResponse>> GetPublishedPostTitlesAsync(PostQuery query)
    {
        var posts = db.Posts
            .Where(p => p.PublishedAt != null && p.PublishedAt <= DateTime.UtcNow);

        if (query.Type is { } type)
            posts = posts.Where(p => p.Type == type);

        if (query.IsFeatured.HasValue)
            posts = posts.Where(p => p.Featured == query.IsFeatured.Value);

        return await posts
            .OrderByDescending(p => p.PublishedAt)
            .Select(p => new PostTitleResponse(p.Slug, p.Title, p.Featured))
            .ToListAsync();
    }
}
