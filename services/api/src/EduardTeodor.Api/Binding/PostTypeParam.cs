using EduardTeodor.Api.Models;

namespace EduardTeodor.Api.Binding;

public readonly record struct PostTypeParam(PostType Value, string? Input, bool IsValid)
{
    public static bool TryParse(string? value, out PostTypeParam result)
    {
        if (Enum.TryParse<PostType>(value, ignoreCase: true, out var type) &&
            Enum.IsDefined(type))
        {
            result = new(type, value, true);
        }
        else
        {
            result = new(default, value, false);
        }

        // Let the endpoint return a structured 400 response instead of failing binding.
        return true;
    }
}
