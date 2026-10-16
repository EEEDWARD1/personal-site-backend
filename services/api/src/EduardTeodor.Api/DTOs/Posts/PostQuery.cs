using EduardTeodor.Api.Models;

namespace EduardTeodor.Api.DTOs.Posts;

public record PostQuery(PostType? Type, bool? IsFeatured);
