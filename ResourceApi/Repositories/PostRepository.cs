using Microsoft.EntityFrameworkCore;
using ResourceApi.Data;
using ResourceApi.DTOs;
using ResourceApi.Models;

namespace ResourceApi.Repositories;

public class PostRepository(AppDbContext db) : IPostRepository
{
    public async Task<List<PostTitleDto>> GetPostTitlesAsync(PostKind? kind, bool? isFeatured)
    {
        var query = db.Posts
            .AsNoTracking()
            .Where(p => p.Published);

        if (kind.HasValue) query = query.Where(p => p.Kind == kind.Value);

        if (isFeatured.HasValue) query = query.Where(p => p.Featured == isFeatured.Value);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostTitleDto(
                p.Slug,
                p.Title,
                p.Featured
            )).ToListAsync();
    }

    public async Task<List<PostSummaryDto>> GetPostSummaryAsync(PostKind? kind, bool? isFeatured)
    {
        var query = db.Posts
            .AsNoTracking()
            .Where(p => p.Published);

        if (kind.HasValue) query = query.Where(p => p.Kind == kind.Value);

        if (isFeatured.HasValue) query = query.Where(p => p.Featured == isFeatured.Value);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostSummaryDto(
                p.Slug,
                p.Title,
                p.Summary,
                p.Featured
            )).ToListAsync();
    }

    public async Task<PostDetailedDto?> GetPostBySlugAsync(string slug)
    {
        return await db.Posts
            .AsNoTracking()
            .Where(p => p.Slug == slug && p.Published)
            .Select(p => new PostDetailedDto(
                p.Slug,
                p.Title,
                p.Summary,
                p.Content,
                p.Featured,
                p.CreatedAt,
                p.PostTags
                    .Select(pt => pt.Tag.Slug)
                    .ToList()
            ))
            .FirstOrDefaultAsync();
    }
}