namespace EduardTeodor.Api.DTOs.Posts;

public record PostDetailedResponse(
    string Slug,
    string Title,
    string Summary,
    string Content,
    DateTime PublishedAt,
    bool Featured
);
