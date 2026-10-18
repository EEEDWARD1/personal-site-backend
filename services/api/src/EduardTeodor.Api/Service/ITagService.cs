using EduardTeodor.Api.DTOs;
using EduardTeodor.Api.DTOs.Tags;

namespace EduardTeodor.Api.Service;
public interface ITagService
{
    Task<PagedResponse<TagResponse>> GetTagsAsync(TagQuery query);
}