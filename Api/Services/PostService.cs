using Api.Data;
using Api.DTOs;
using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class PostService(AppDbContext db)
{
    public record PagedResult<T>(List<T> Items, int Total);
    
    private IQueryable<Post> BuildFilteredQuery(PostKind kind, PostListQuery q)
    {
        var query = db.Posts
            .AsNoTracking()
            .Where(p => p.Kind == kind && p.Published);

        if (q.Featured is bool featured)
            query = query.Where(p => p.Featured == featured);

        if (!string.IsNullOrWhiteSpace(q.Tag))
            query = query.Where(p => p.PostTags.Any(pt => pt.Tag.Slug == q.Tag));

        return query;
    }

    private static IQueryable<Post> ApplyOrderAndPaging(IQueryable<Post> query, PostListQuery q)
    {
        var ordered = q.Sort == "featured"
            ? query.OrderByDescending(p => p.Featured)
                .ThenByDescending(p => p.PublishedAt)
                .ThenBy(p => p.Slug)
            : query.OrderByDescending(p => p.PublishedAt)
                .ThenBy(p => p.Slug);

        return ordered.Skip(q.EffectiveOffset).Take(q.EffectiveLimit);
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

    public async Task<PagedResult<PublicPostSummaryResponse>> ListPublishedAsync(
    PostKind kind, PostListQuery q, CancellationToken ct)
    {
        var filtered = BuildFilteredQuery(kind, q);

        var total = await filtered.CountAsync(ct);

        var items = await ApplyOrderAndPaging(filtered, q)
            .Select(p => new PublicPostSummaryResponse(
                p.Slug, p.Title, p.Summary, p.Featured, p.PublishedAt,
                p.PostTags.Select(pt => pt.Tag.Name).ToList()))
            .ToListAsync(ct);

        return new(items, total);
    }

    public async Task<PagedResult<PublicPostSlugResponse>> ListPublishedSlugsAsync(
    PostKind kind, PostListQuery q, CancellationToken ct)
    {
        var filtered = BuildFilteredQuery(kind, q);

        var total = await filtered.CountAsync(ct);

        var items = await ApplyOrderAndPaging(filtered, q)
            .Select(p => new PublicPostSlugResponse(p.Slug))
            .ToListAsync(ct);

        return new(items, total);
    }

    
}