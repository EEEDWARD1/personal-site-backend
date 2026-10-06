namespace Api.DTOs;

public record PublicPostResponse(
    string Slug,
    string Title,
    string? Summary,
    string Content,
    bool Featured,
    DateTimeOffset? PublishedAt,
    IReadOnlyList<string> Tags
);