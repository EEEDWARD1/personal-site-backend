using EduardTeodor.Api.Binding;
using EduardTeodor.Api.Models;

namespace EduardTeodor.Api.DTOs.Requests.Posts;

public record PostQuery(EnumParam<PostType>? Type, bool? IsFeatured);