using EduardTeodor.Api.DTOs;
using EduardTeodor.Api.DTOs.Posts;
using EduardTeodor.Api.Models;
using EduardTeodor.Api.Repositories;

namespace EduardTeodor.Api.Service;

public class AdminPostService(IAdminPostRepository repository) : IAdminPostService
{
    public async Task<(AdminPostDetailedResponse Post, uint Version)?> GetByIdAsync(Guid id)
    {
        var post = await repository.GetByIdAsync(id);
        return post is null ? null : (new AdminPostDetailedResponse(
            post.Id, post.Slug, post.Title, post.Content, post.Summary, post.Type,
            post.Featured, post.PublishedAt, post.CreatedAt, post.UpdatedAt), post.Version);
    }

    public Task<PostUpdateStatus> UpdateAsync(Guid id, UpdatePostRequest request, uint expectedVersion) =>
        repository.UpdateAsync(new Post
        {
            Id = id,
            Slug = request.Slug,
            Title = request.Title.Trim(),
            Content = request.Content,
            Summary = request.Summary,
            Type = request.Type,
            Featured = request.Featured,
            PublishedAt = request.PublishedAt?.UtcDateTime
        }, expectedVersion);

    public async Task<PagedResponse<AdminPostTitleResponse>> GetTitlesAsync(PageQuery query)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var skip = (int)Math.Min(((long)page - 1) * pageSize, int.MaxValue);
        var (items, totalCount) = await repository.GetTitlesAsync(skip, pageSize);

        return new PagedResponse<AdminPostTitleResponse>(
            items.Select(p => new AdminPostTitleResponse(p.Id, p.Title)).ToList(), page, pageSize, totalCount);
    }

    public async Task<AdminPostTitleResponse?> CreateAsync(CreatePostRequest request)
    {
        var post = new Post
        {
            Slug = request.Slug,
            Title = request.Title.Trim(),
            Content = request.Content,
            Summary = request.Summary ?? "",
            Type = request.Type,
            Featured = request.Featured,
            PublishedAt = request.PublishedAt?.UtcDateTime
        };

        return await repository.CreateAsync(post) ? new AdminPostTitleResponse(post.Id, post.Title) : null;
    }

    public Task<bool> DeleteAsync(Guid id) => repository.DeleteAsync(id);
}
