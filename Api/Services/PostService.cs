using Api.Data;
using Api.DTOs;
using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class PostService(AppDbContext db)
{
    private IQueryable<Post> BuildPublishedQuery(PostKind kind, PostListQuery q)
    {
        var query = db.Posts
            .AsNoTracking()
            .Where(p => p.Kind == kind && p.Published);

        if (q.Featured is bool featured)
            query = query.Where(p => p.Featured == featured);

        query = q.Sort == "featured"
            ? query.OrderByDescending(p => p.Featured)
                .ThenByDescending(p => p.PublishedAt)
                .ThenBy(p => p.Slug)
            : query.OrderByDescending(p => p.PublishedAt)
                .ThenBy(p => p.Slug);

        return query.Skip(q.EffectiveOffset).Take(q.EffectiveLimit);
    }
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

    public async Task<List<PublicPostSummaryResponse>> ListPublishedAsync(
    PostKind kind, PostListQuery q, CancellationToken ct) =>
    await BuildPublishedQuery(kind, q)
        .Select(p => new PublicPostSummaryResponse(
            p.Slug, p.Title, p.Summary, p.Featured, p.PublishedAt,
            p.PostTags.Select(pt => pt.Tag.Name).ToList()))
        .ToListAsync(ct);

    public async Task<List<PublicPostSlugResponse>> ListPublishedSlugsAsync(
        PostKind kind, PostListQuery q, CancellationToken ct) =>
        await BuildPublishedQuery(kind, q)
            .Select(p => new PublicPostSlugResponse(p.Slug))
            .ToListAsync(ct);

    
}