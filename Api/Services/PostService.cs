using Api.Data;
using Api.DTOs;
using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class PostService(AppDbContext db)
{
    public async Task<PublicPostResponse?> GetPublishedBySlugAsync(PostKind kind, string slug, CancellationToken ct)
    {
        return await db.Posts
            .AsNoTracking()
            .Where(p => p.Kind == kind
                && p.Published
                && p.Slug == slug)
            .Select(p => new PublicPostResponse(
                p.Slug,
                p.Title,
                p.Summary,
                p.Content,
                p.Featured,
                p.PublishedAt,
                p.PostTags.Select(pt => pt.Tag.Name).ToList()))
            .FirstOrDefaultAsync(ct);
    }
}