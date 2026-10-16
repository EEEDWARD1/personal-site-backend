namespace EduardTeodor.Api.DTOs.Responses.Posts;

public record PostSummaryResponse(
    string Slug,
    string Title,
    string Summary,
    bool Featured
);