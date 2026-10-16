using EduardTeodor.Api.Binding;

namespace EduardTeodor.Api.DTOs.Posts;

public record PostQuery(PostTypeParam? Type, bool? IsFeatured);
