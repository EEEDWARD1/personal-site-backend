namespace EduardTeodor.Api.DTOs.Responses.Posts;

public record PostTitleResponse(
    string Slug,
    string Title,
    bool Featured
);