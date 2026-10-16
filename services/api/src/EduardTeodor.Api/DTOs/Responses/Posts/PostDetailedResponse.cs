using System.Text.Json.Serialization;

namespace EduardTeodor.Api.DTOs.Responses.Posts;

public record PostDetailedResponse(
    string Slug,
    string Title,
    string Summary,
    string Content,
    DateTime PublishedAt,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? RepoUrl,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? LiveUrl,
    bool Featured
);