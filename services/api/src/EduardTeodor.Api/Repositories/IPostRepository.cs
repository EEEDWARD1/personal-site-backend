using EduardTeodor.Api.Models;

namespace EduardTeodor.Api.Repositories;

public interface IPostRepository
{
    Task<Post?> GetPublishedPostBySlugAsync(string slug);
    Task<(IReadOnlyList<PostListItem> Items, int TotalCount)> GetPublishedPageAsync(
        int skip,
        int take,
        PostType? type,
        bool? isFeatured,
        string[] tagSlugs,
        bool includeTags);
}
