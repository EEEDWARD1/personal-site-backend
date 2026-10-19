using EduardTeodor.Api.DTOs;
using EduardTeodor.Api.DTOs.Posts;
using EduardTeodor.Api.Models;

namespace EduardTeodor.Api.Service;

public interface IAdminPostService
{
    Task<(AdminPostDetailedResponse Post, uint Version)?> GetByIdAsync(Guid id);
    Task<PostUpdateStatus> UpdateAsync(Guid id, UpdatePostRequest request, uint expectedVersion);
    Task<PagedResponse<AdminPostTitleResponse>> GetTitlesAsync(PageQuery query);
    Task<AdminPostTitleResponse?> CreateAsync(CreatePostRequest request);
    Task<bool> DeleteAsync(Guid id);
}
