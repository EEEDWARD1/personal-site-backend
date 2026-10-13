using ResourceApi.DTOs;
using ResourceApi.Models;

public interface IPostRepository
{
    Task<PostDetailedDto?> GetPostBySlugAsync(string slug);
    Task<List<PostSummaryDto>> GetPostSummaryAsync(PostKind? parsedKind, bool? isFeatured);
    Task<List<PostTitleDto>> GetPostTitlesAsync(PostKind? kind, bool? isFeatured);
}