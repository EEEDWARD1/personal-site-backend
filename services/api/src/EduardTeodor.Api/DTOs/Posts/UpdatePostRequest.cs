using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EduardTeodor.Api.Models;
using EduardTeodor.Api.Validation;

namespace EduardTeodor.Api.DTOs.Posts;

public record UpdatePostRequest(
    [property: JsonRequired, Required, PostSlug, RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")] string Slug,
    [property: JsonRequired, Required] string Title,
    [property: JsonRequired, Required] string Content,
    [property: JsonRequired, Required(AllowEmptyStrings = true)] string Summary,
    [property: JsonRequired, EnumDataType(typeof(PostType))] PostType Type,
    [property: JsonRequired] bool Featured,
    [property: JsonRequired] DateTimeOffset? PublishedAt);
