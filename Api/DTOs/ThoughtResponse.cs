namespace Api.DTOs;

public record ThoughtResponse(
    string Slug,
    string Title,
    string? Summary,
    string Content,
    bool Featured,
    DateTimeOffset? PublishedAt,
    IReadOnlyList<string> Tags
);