namespace EduardTeodor.Api.Binding;

/// <summary>
/// Query-string wrapper that binds an enum case-insensitively. Minimal APIs bind any
/// type with a static TryParse; failure yields 400. Numeric values are rejected.
/// </summary>
public readonly record struct EnumParam<T>(T Value) where T : struct, Enum
{
    public static bool TryParse(string? s, out EnumParam<T> result)
    {
        foreach (var name in Enum.GetNames<T>())
        {
            if (string.Equals(name, s, StringComparison.OrdinalIgnoreCase))
            {
                result = new(Enum.Parse<T>(name));
                return true;
            }
        }

        result = default;
        return false;
    }
}
