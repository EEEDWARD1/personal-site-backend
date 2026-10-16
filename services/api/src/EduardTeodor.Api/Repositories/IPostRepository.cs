using EduardTeodor.Api.DTOs;
using EduardTeodor.Api.DTOs.Posts;
using EduardTeodor.Api.Models;

public interface IPostRepository
{
    Task<PostDetailedResponse?> GetPublishedPostBySlugAsync(string slug);
    Task<PagedResponse<PostSummaryResponse>> GetPublishedPostSummariesAsync(PostQuery query);
    Task<PagedResponse<PostTitleResponse>> GetPublishedPostTitlesAsync(PostQuery query);
}
