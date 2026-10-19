using EduardTeodor.Api.Models;

namespace EduardTeodor.Api.DTOs.Posts;

public record AdminPostDetailedResponse(
    Guid Id,
    string Slug,
    string Title,
    string Content,
    string Summary,
    PostType Type,
    bool Featured,
    DateTime? PublishedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt);
