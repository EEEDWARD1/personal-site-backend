namespace Api.DTOs;

public record PublicPostSummaryResponse(
    string Slug,
    string Title,
    string? Summary,
    bool Featured,
    DateTimeOffset? PublishedAt,
    IReadOnlyList<string> Tags
);