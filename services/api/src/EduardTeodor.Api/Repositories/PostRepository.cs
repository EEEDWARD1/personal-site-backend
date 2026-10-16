using EduardTeodor.Api.Data;
using EduardTeodor.Api.DTOs.Requests.Posts;
using EduardTeodor.Api.DTOs.Responses.Posts;
using Microsoft.EntityFrameworkCore;

namespace EduardTeodor.Api.Repositories;

public class PostRepository(AppDbContext db) : IPostRepository
{
    public async Task<List<PostTitleResponse>> GetPublishedPostTitlesAsync(PostQuery query)
    {
        var posts = db.Posts
            .Where(p => p.PublishedAt != null && p.PublishedAt <= DateTime.UtcNow);

        if (query.Type.HasValue)
            posts = posts.Where(p => p.Type == query.Type.Value.Value);

        if (query.IsFeatured.HasValue)
            posts = posts.Where(p => p.Featured == query.IsFeatured.Value);

        return await posts
            .OrderByDescending(p => p.PublishedAt)
            .Select(p => new PostTitleResponse(p.Slug, p.Title, p.Featured))
            .ToListAsync();
    }
}