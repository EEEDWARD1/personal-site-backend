using Api.Data;
using Api.DTOs;
using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class ProjectService(AppDbContext db)
{
    public async Task<ProjectResponse?> GetBySlugAsync(string slug, CancellationToken ct)
    {
        return await db.Posts
            .AsNoTracking()
            .Where(p => p.Kind == PostKind.Project
                && p.Published
                && p.Slug == slug)
            .Select(p => new ProjectResponse(
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