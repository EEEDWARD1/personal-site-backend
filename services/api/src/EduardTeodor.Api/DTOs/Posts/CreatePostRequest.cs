using System.ComponentModel.DataAnnotations;
using EduardTeodor.Api.Models;
using EduardTeodor.Api.Validation;

namespace EduardTeodor.Api.DTOs.Posts;

public record CreatePostRequest(
    [property: Required, PostSlug, RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")] string Slug,
    [property: Required] string Title,
    [property: Required] string Content,
    string Summary = "",
    [property: EnumDataType(typeof(PostType))] PostType Type = PostType.Thought,
    bool Featured = false,
    DateTimeOffset? PublishedAt = null);
