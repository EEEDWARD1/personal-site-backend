namespace EduardTeodor.Api.DTOs.Posts;

public record PostSummaryResponse(
    string Slug,
    string Title,
    string Summary,
    bool Featured
);
