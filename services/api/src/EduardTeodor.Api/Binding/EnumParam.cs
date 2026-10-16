namespace EduardTeodor.Api.Binding;

/// <summary>
/// Query-string wrapper that binds an enum case-insensitively. Unknown or numeric
/// values bind as <c>Value = null</c>, i.e. "no filter", instead of failing with 400.
/// </summary>
public readonly record struct EnumParam<T>(T? Value) where T : struct, Enum
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
        return true;
    }
}
