using EduardTeodor.Api.Data;
using EduardTeodor.Api.Models;

namespace EduardTeodor.Api.Repositories;

public interface ITagRepository
{
    //Task<PagedResponse<TagResponse>> GetTagsAsync(TagQuery query);
    Task<(IReadOnlyList<Tag> Items, int TotalCount)> GetPageAsync(int skip, int take);
}
