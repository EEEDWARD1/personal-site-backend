using EduardTeodor.Api.Models;

namespace EduardTeodor.Api.Binding;

public readonly record struct PostTypeParam(PostType Value)
{
    public static bool TryParse(string? value, out PostTypeParam result)
    {
        if (Enum.TryParse<PostType>(value, ignoreCase: true, out var type) &&
            Enum.IsDefined(type))
        {
            result = new(type);
            return true;
        }

        result = default;
        return false;
    }
}
