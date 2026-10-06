namespace Api.DTOs;

public record PostListQuery(bool? Featured, string? Sort, int? Limit, int? Offset, string? Tag)
{
    public const int DefaultLimit = 20;
    public const int MaxLimit = 50;

    public int EffectiveLimit => Limit ?? DefaultLimit;
    public int EffectiveOffset => Offset ?? 0;

    public ApiError? Validate()
    {
        if (Sort is not (null or "recent" or "featured"))
            return new ApiError(new ErrorBody("INVALID_QUERY", "sort must be 'recent' or 'featured'."));

        if (EffectiveLimit is < 1 or > MaxLimit)
            return new ApiError(new ErrorBody("INVALID_QUERY", $"limit must be between 1 and {MaxLimit}."));

        if (EffectiveOffset < 0)
            return new ApiError(new ErrorBody("INVALID_QUERY", "offset must be 0 or greater."));
        if (Tag is not null && Tag.Length > 50)
            return new ApiError(new ErrorBody("INVALID_QUERY", "tag must be 50 characters or less."));

        return null;
    }
}