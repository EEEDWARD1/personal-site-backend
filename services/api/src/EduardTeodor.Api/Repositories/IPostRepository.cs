using EduardTeodor.Api.DTOs.Requests.Posts;
using EduardTeodor.Api.DTOs.Responses.Posts;
using EduardTeodor.Api.Models;

public interface IPostRepository
{
    Task<List<PostTitleResponse>> GetPublishedPostTitlesAsync(PostQuery query);
}