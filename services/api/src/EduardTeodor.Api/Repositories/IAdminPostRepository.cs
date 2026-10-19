using EduardTeodor.Api.Models;

namespace EduardTeodor.Api.Repositories;

public interface IAdminPostRepository
{
    Task<Post?> GetByIdAsync(Guid id);
    Task<PostUpdateStatus> UpdateAsync(Post post, uint expectedVersion);
    Task<(IReadOnlyList<(Guid Id, string Title)> Items, int TotalCount)> GetTitlesAsync(int skip, int take);
    Task<bool> CreateAsync(Post post);
    Task<bool> DeleteAsync(Guid id);
}
