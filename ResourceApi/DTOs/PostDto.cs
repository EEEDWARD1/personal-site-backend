namespace ResourceApi.DTOs;

public record PostTitleDto(
    string Slug,
    string Title,
    bool Featured
);

public record PostSummaryDto(
    string Slug,
    string Title,
    string? Summary,
    bool Featured
);

public record PostDetailedDto(
    string Slug,
    string Title,
    string? Summary,
    string Content,
    bool Featured,
    DateTime PublishedAt,
    List<string> Tags
);