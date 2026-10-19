using EduardTeodor.Api.Data;
using EduardTeodor.Api.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EduardTeodor.Api.Repositories;

public class AdminPostRepository(AppDbContext db) : IAdminPostRepository
{
    public Task<Post?> GetByIdAsync(Guid id) =>
        db.Posts.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);

    public async Task<PostUpdateStatus> UpdateAsync(Post post, uint expectedVersion)
    {
        try
        {
            var affected = await db.Posts.Where(p => p.Id == post.Id && p.Version == expectedVersion)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.Slug, post.Slug)
                    .SetProperty(p => p.Title, post.Title)
                    .SetProperty(p => p.Content, post.Content)
                    .SetProperty(p => p.Summary, post.Summary)
                    .SetProperty(p => p.Type, post.Type)
                    .SetProperty(p => p.Featured, post.Featured)
                    .SetProperty(p => p.PublishedAt, post.PublishedAt)
                    .SetProperty(p => p.UpdatedAt, DateTime.UtcNow));

            if (affected > 0)
                return PostUpdateStatus.Updated;

            return await db.Posts.AnyAsync(p => p.Id == post.Id)
                ? PostUpdateStatus.VersionConflict
                : PostUpdateStatus.NotFound;
        }
        catch (PostgresException ex) when (ex is
               { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "posts_slug_key" })
        {
            return PostUpdateStatus.DuplicateSlug;
        }
    }

    public async Task<(IReadOnlyList<(Guid Id, string Title)> Items, int TotalCount)> GetTitlesAsync(int skip, int take)
    {
        var posts = db.Posts.AsNoTracking();
        var totalCount = await posts.CountAsync();
        var items = await posts.OrderByDescending(p => p.CreatedAt).ThenBy(p => p.Id)
            .Skip(skip).Take(take)
            .Select(p => new { p.Id, p.Title })
            .ToListAsync();

        return (items.Select(p => (p.Id, p.Title)).ToList(), totalCount);
    }

    public async Task<bool> CreateAsync(Post post)
    {
        db.Posts.Add(post);
        try
        {
            await db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException
               { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "posts_slug_key" })
        {
            db.Entry(post).State = EntityState.Detached;
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid id) =>
        await db.Posts.Where(p => p.Id == id).ExecuteDeleteAsync() > 0;
}
