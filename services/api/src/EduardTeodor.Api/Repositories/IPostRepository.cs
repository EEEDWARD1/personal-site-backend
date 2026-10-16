using EduardTeodor.Api.DTOs.Posts;
using EduardTeodor.Api.Models;

public interface IPostRepository
{
    Task<PostDetailedResponse?> GetPublishedPostBySlugAsync(string slug);
    Task<List<PostSummaryResponse>> GetPublishedPostSummariesAsync(PostQuery query);
    Task<List<PostTitleResponse>> GetPublishedPostTitlesAsync(PostQuery query);
}
