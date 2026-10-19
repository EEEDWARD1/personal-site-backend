using EduardTeodor.Api.DTOs;
using EduardTeodor.Api.DTOs.Tags;
using EduardTeodor.Api.Repositories;

namespace EduardTeodor.Api.Service;
public class TagService(ITagRepository repository) : ITagService
{
    private readonly ITagRepository _repository = repository;

    public async Task<PagedResponse<TagResponse>> GetTagsAsync(TagQuery query)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize,1,100);
        var skip = (int)Math.Min(((long)page - 1) * pageSize, int.MaxValue);
        var (tags, totalCount) = await 
            _repository.GetPageAsync(skip, pageSize);

        var items = tags
            .Select(t => new TagResponse(
                t.Name, 
                t.Slug))
            .ToList();

        return new PagedResponse<TagResponse>(
            items, 
            page, 
            pageSize, 
            totalCount);
        
    }
}