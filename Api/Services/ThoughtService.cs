using Api.Data;
using Api.DTOs;
using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class ThoughtService(AppDbContext db)
{
    public async Task<ThoughtResponse?> GetBySlugAsync(string slug, CancellationToken ct)
    {
        return await db.Posts
            .AsNoTracking()
            .Where(p => p.Kind == PostKind.Thought
                && p.Published
                && p.Slug == slug)
            .Select(p => new ThoughtResponse(
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