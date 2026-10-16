namespace EduardTeodor.Api.DTOs.Posts;

public record PostQuery(
    string? Type,
    bool? IsFeatured,
    bool? EnableTags,
    string[]? Tag = null,
    int Page = 1,
    int PageSize =20
);
