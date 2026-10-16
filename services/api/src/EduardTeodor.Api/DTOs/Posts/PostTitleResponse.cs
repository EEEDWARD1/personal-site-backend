namespace EduardTeodor.Api.DTOs.Posts;

public record PostTitleResponse(
    string Slug,
    string Title,
    bool Featured
);
