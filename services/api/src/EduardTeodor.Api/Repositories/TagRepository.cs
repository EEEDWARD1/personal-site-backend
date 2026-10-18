using EduardTeodor.Api.Data;
using EduardTeodor.Api.Models;
using Microsoft.EntityFrameworkCore;


namespace EduardTeodor.Api.Repositories;

public class TagRepository(AppDbContext context) : ITagRepository
{
    /**public async Task<PagedResponse<TagResponse>> GetTagsAsync(TagQuery query)
    {
        var tags = db.Tags.AsNoTracking();
        var totalCount = await tags.CountAsync();

        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var skip = (int)Math.Min(((long)page - 1) * pageSize, int.MaxValue);

        var items = await tags
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(t => new TagResponse(t.Name, t.Slug))
            .ToListAsync();

        return new PagedResponse<TagResponse>(items, page, pageSize, totalCount);
    }**/

    private readonly AppDbContext _context = context;

    public async Task<(IReadOnlyList<Tag> Items, int TotalCount)> GetPageAsync(
        int skip, 
        int take)
    {
        var tags = _context.Tags.AsNoTracking();

        var totalCount = await tags.CountAsync();

        var items = await tags
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
        
        return(items, totalCount);
    }
}
