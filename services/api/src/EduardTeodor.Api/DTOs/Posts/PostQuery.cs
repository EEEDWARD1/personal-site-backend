using EduardTeodor.Api.Binding;

namespace EduardTeodor.Api.DTOs.Posts;

public record PostQuery(
    PostTypeParam? Type,
    bool? IsFeatured,
    bool? EnableTags,
    string[]? Tag = null,
    int Page = 1,
    int PageSize =20
);
