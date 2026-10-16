using EduardTeodor.Api.DTOs;
using EduardTeodor.Api.DTOs.Tags;

namespace EduardTeodor.Api.Repositories;

public interface ITagRepository
{
    Task<PagedResponse<TagResponse>> GetTagsAsync(TagQuery query);
}
