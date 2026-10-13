using ResourceApi.DTOs;
using ResourceApi.Models;

public interface IPostRepository
{
    Task<PostDetailedDto?> GetPostBySlugAsync(string slug);
    Task<List<PostSummaryDto>> GetPostSummaryAsync(bool? isProject, bool? isFeatured);
    Task<List<PostTitleDto>> GetPostTitlesAsync(bool? isProject, bool? isFeatured);
    Task<String> CreatePostAsync(CreatePostDto request);
}