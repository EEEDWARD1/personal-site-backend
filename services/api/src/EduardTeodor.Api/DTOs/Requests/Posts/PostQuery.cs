using EduardTeodor.Api.Models;

namespace EduardTeodor.Api.DTOs.Requests.Posts;

public record PostQuery(PostType? Type, bool? IsFeatured);