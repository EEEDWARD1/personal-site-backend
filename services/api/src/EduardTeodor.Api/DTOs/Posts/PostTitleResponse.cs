using System.Text.Json.Serialization;
using EduardTeodor.Api.DTOs.Tags;

namespace EduardTeodor.Api.DTOs.Posts;

public record PostTitleResponse(
    string Slug,
    string Title,
    bool Featured,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] List<TagResponse>? Tags = null
);
