using System.Text.Json.Serialization;
using EduardTeodor.Api.DTOs.Tags;

namespace EduardTeodor.Api.DTOs.Posts;

public record PostSummaryResponse(
    string Slug,
    string Title,
    string Summary,
    bool Featured,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] List<TagResponse>? Tags = null
);
